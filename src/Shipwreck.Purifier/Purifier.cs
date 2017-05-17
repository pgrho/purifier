using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shipwreck.Purifier
{
    public static class Purifier
    {
        private const string HAN = @"[\u2e80-\u2fdf\u3400-\u4dbf\u4d00-\u9fff\uf900-\ufaff\ud800-\udfff]";
        private const string PRONOUN_PREFIX = "((?<!" + HAN + ")|^)";
        private const string PRONOUN_SUFFIX = "((?!" + HAN + ")|$)";
        private static readonly Regex we = new Regex(PRONOUN_PREFIX + "((私|僕)(達|たち)|我ら|我々)" + PRONOUN_SUFFIX);
        private static readonly Regex me = new Regex(PRONOUN_PREFIX + "(我|僕)" + PRONOUN_SUFFIX);
        private static readonly Regex comma = new Regex("[､、]");

        private const string DOT_CORE = "[～…]*[｡。!?！？｣」》♪♫]";
        private const string DOT =
            "("
            + "(?<ne>ね)"
            + "|(?<yo>(だ|こと)?よ)" // TODO: 連用形+てよ
            + "|(?<remove>です|だ|だ?わ|さ|な|の)" // TODO: こと|ものが体言止めかどうか
            + ")?(?<dot>" + DOT_CORE + "+|" + DOT_CORE + "*$)";

        private static readonly Regex dot = new Regex(DOT);

        public static string Purify(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            int ws = 0, ms = 0, cs = 0, ds = 0;

            while (ws >= 0 || ms >= 0 || cs >= 0 || ds >= 0)
            {
                var wm = GetMatch(we, text, ws);
                var mm = GetMatch(me, text, ms);
                var cm = GetMatch(comma, text, cs);
                var dm = GetMatch(dot, text, ds);

                Match used = null;
                var dl = 0;

                if (wm?.Success == true && IsPriorTo(wm, mm) && IsPriorTo(wm, cm) && IsPriorTo(wm, dm))
                {
                    used = wm;

                    text = wm.Result("$`私たち$'");
                    dl = 3 - wm.Length;
                }
                else if (mm?.Success == true && IsPriorTo(mm, cm) && IsPriorTo(mm, dm))
                {
                    used = mm;

                    text = mm.Result("$`私$'");
                    dl = 1 - mm.Length;
                }
                else if (cm?.Success == true && IsPriorTo(cm, dm))
                {
                    used = cm;

                    // TODO: 要検討
                }
                else if (dm?.Success == true)
                {
                    used = dm;
                    var isLast = text.Length == dm.Index + dm.Length;
                    text = dm.Result(GetDotReplacement(text, dm, out dl));
                    if (isLast)
                    {
                        break;
                    }
                }

                ws = GetNextStartIndex(wm, used, dl);
                ms = GetNextStartIndex(mm, used, dl);
                cs = GetNextStartIndex(cm, used, dl);
                ds = GetNextStartIndex(dm, used, dl);
            }

            return text;
        }

        private static Match GetMatch(Regex regex, string text, int startAt)
            => 0 <= startAt && startAt <= text.Length ? regex.Match(text, startAt) : null;

        private static bool IsPriorTo(Match m, Match other)
            => !(other?.Success == true && m.Index > other.Index);

        private static int GetNextStartIndex(Match m, Match usedMatch, int deltaLength)
            => m?.Success != true ? -1 : (m.Index + deltaLength + (usedMatch == m ? m.Length : 0));

        private static string GetDotReplacement(string text, Match dm, out int dl)
        {
            var dotChar = dm.Value.FirstOrDefault();
            var exclamation = dotChar == '!' || dotChar == '！';

            var ng = dm.Groups["ne"];
            if (ng.Success)
            {
                dl = 2;
                return "$`ぷりね${dot}$'";
            }
            var yg = dm.Groups["yo"];
            if (yg.Success)
            {
                dl = 3 - yg.Length;
                return "$`ぷりよ${dot}$'";
            }
            var rg = dm.Groups["remove"];
            if (rg.Success)
            {
                dl = 2 - rg.Length;
                return "$`ぷり${dot}$'";
            }

            // TODO: 敬語対策
            // TODO: である等の削除

            if (exclamation)
            {
                dl = 3;
                return "$`っぷり$&$'";
            }

            dl = 2;
            return "$`ぷり$&$'";
        }
    }
}
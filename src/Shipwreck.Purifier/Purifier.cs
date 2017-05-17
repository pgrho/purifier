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
        private static readonly Regex we = new Regex("(私達|我々|われわれ)");
        private static readonly Regex me = new Regex("僕");
        private static readonly Regex comma = new Regex("[､、]");

        private const string DOT_CORE = "[｡。!?！？]";
        private const string DOT =
            "("
            + "(?<ne>ね)"
            + "|(?<yo>(だ|こと)?よ)" // TODO: 連用形+てよ
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

                    if (IsPronoun(text, wm))
                    {
                        text = wm.Result("$`私たち$'");
                        dl = 3 - wm.Length;
                    }
                }
                else if (mm?.Success == true && IsPriorTo(mm, cm) && IsPriorTo(mm, dm))
                {
                    used = mm;

                    if (IsPronoun(text, mm))
                    {
                        text = mm.Result("$`私$'");
                        dl = 1 - mm.Length;
                    }
                }
                else if (cm?.Success == true && IsPriorTo(cm, dm))
                {
                    used = cm;

                    // TODO: 実装
                }
                else if (dm?.Success == true)
                {
                    used = dm;
                    text = dm.Result(GetDotReplacement(text, dm, out dl));
                }

                ws = GetNextStartIndex(wm, used, dl);
                ms = GetNextStartIndex(mm, used, dl);
                cs = GetNextStartIndex(cm, used, dl);
                ds = GetNextStartIndex(dm, used, dl);
            }

            return text;
        }

        private static Match GetMatch(Regex regex, string text, int startAt)
            => 0 <= startAt && startAt < text.Length ? regex.Match(text, startAt) : null;

        private static bool IsPriorTo(Match m, Match other)
            => !(other?.Success == true && m.Index > other.Index);

        private static int GetNextStartIndex(Match m, Match usedMatch, int deltaLength)
            => m?.Success != true ? -1 : (m.Index + deltaLength + (usedMatch == m ? m.Length : 0));

        private static bool IsPronoun(string text, Match m)
        {
            // TODO: 代名詞を置換可能か判定する
            return true;
        }

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

            // TODO: 敬語対策
            // TODO: です|である等の削除

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
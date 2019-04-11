using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoSmart.Unicode;

namespace stats_eba_bot.Helpers
{
    public static class EmojiHelper
    {
       

        public static string GeneratePositionEmoju(int position)
        {
            if (position == 0) return Emoji.Crown.ToString();
            if (position == 1) return Emoji.GemStone.ToString();
            if (position == 2) return Emoji.ThirdPlaceMedal.ToString();
            return Emoji.PileOfPoo.ToString();
        }
    }
}

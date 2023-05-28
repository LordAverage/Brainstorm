using System.Collections.Generic;

namespace Brainstorm.src
{
    internal class Decision
    {
        public int MaxCaptures { get; set; }
        public List<Direction> BestPositionList { get; set; }
    }
}

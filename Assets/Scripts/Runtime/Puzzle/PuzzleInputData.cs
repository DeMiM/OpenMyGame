namespace Test.Puzzle
{
    [System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8 )]
    public struct PuzzleInputData
    {
        public readonly UnityEngine.Sprite sprite;
        public PuzzleType puzzleType;

        public PuzzleInputData( UnityEngine.Sprite sprite )
        {
            this.sprite = sprite;
            puzzleType = PuzzleType.Puzzle4;
        }
    }

}

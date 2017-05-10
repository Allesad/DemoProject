namespace YumaPos.Core
{
    public class Calculator
    {
        private int _result;

        public Calculator()
        {
            _result = 0;
        }

        public int Add(int toAdd)
        {
            _result += toAdd;
            return _result;
        }

        public int CalculateResult()
        {
            return _result;
        }
    }
}
using static System.Math;
namespace GPImplemenation
{
    public class Function
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Arity { get; set; }

        public static double Evaluate(int functionId, params double[] tt)
        {
            switch (functionId)
            {
                case 0:
                    {
                        return tt[0] + tt[1];
                    }
                case 1:
                    {
                        return tt[0] - tt[1];
                    }
                case 2:
                    {
                        return tt[0] * tt[1];
                    }
                case 3://protected division
                    {
                        if (tt[1] == 0)
                            return 0;
                        return tt[0] / tt[1];
                    }
                case 4:
                    {
                        if (tt[0] == 0)
                            return 0;
                        return 1.0 / tt[0];
                    }
                case 5:
                    {
                        return Sin(tt[0]);
                    }
                case 6:
                    {
                        return Tan(tt[0]);
                    }
                default:
                    {
                        return double.NaN;
                    }
            }
        }

        public static string GetName(int functionId, params string[] tt)
        {
            switch (functionId)
            {
                case 0:
                    {
                        return $"({tt[0]} + {tt[1]})";
                    }
                case 1:
                    {
                        return $"({tt[0]} - {tt[1]})";
                    }
                case 2:
                    {
                        return $"({tt[0]} * {tt[1]})";
                    }
                case 3:
                    {
                        return $"({tt[0]} / {tt[1]})";
                    }
                case 4:
                    {
                        return $"(1 / {tt[0]})";
                    }
                case 5:
                    {
                        return $"Sin({tt[0]})";
                    }
                case 6:
                    {
                        return $"Tan({tt[0]})";
                    }
                default:
                    {
                        return " ";
                    }
            }
        }
    }
}

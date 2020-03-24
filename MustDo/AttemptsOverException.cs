using System;

namespace MustDo
{
    public class AttemptsOverException : Exception
    {
        public AttemptsOverException(Exception e) : base("Attempts over", e)
        {

        }
    }
}
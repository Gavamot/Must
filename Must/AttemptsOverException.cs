using System;

namespace Must
{
    public class AttemptsOverException : Exception
    {
        public AttemptsOverException(Exception e) : base("Attempts over", e)
        {

        }
    }
}
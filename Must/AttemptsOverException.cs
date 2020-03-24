using System;

namespace ConsoleApp6
{
    public class AttemptsOverException : Exception
    {
        public AttemptsOverException(Exception e) : base("Attempts over", e)
        {

        }
    }
}
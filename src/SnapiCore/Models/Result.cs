using System;

namespace SnapiCore.Models
{
    
    public class Result<TStatus, TValue> where TStatus : Enum
    {
        public TStatus Status { get; set; }
        public TValue Value { get; set; }

        public static implicit operator Result<TStatus, TValue>((TStatus status, TValue value) data)
        {
            var (status, value) = data;
            return new Result<TStatus, TValue>()
            {
                Status = status,
                Value = value
            };
        }
    }
}
﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataWorkerService.Models
{
    
    public class Result
    {
        private int SuccessCode = 200;
        public Result(int code = 200, string message = "")
        {
            Code = code;
            Message = message;
        }

        public object Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public static Result Success(string message = "") => new Result(200, message);

        public static Result Fail(int code, string message = "Didn't connect to device. Please connect to device first!") => new Result(code, message);

        public bool IsSuccess => Code == SuccessCode;

        public string ToString() => JsonConvert.SerializeObject(this);
    }
}

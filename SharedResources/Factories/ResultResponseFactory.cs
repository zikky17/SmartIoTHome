using SharedResources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Factories
{
    public static class ResultResponseFactory
    {
        public static ResultResponse Success(string message)
        {
            return new ResultResponse
            {
                Succeeded = true,
                Message = message
            };
        }

        public static ResultResponse Failed(string message)
        {
            return new ResultResponse
            {
                Succeeded = false,
                Message = message
            };
        }
    }

    public static class ResultResponseFactory<T>
    {
        public static ResultResponse<T> Success(string message = default!, T? content = default)
        {
            return new ResultResponse<T>
            {
                Succeeded = true,
                Content = content,
                Message = message
            };
        }

        public static ResultResponse<T> Failed(string message = default!, T content = default!)
        {
            return new ResultResponse<T>
            {
                Succeeded = false,
                Content = content,
                Message = message
            };
        }
    }
}
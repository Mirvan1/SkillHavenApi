using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Services    
{
    public interface ILoggerService<T>
    {
        public void LogDebug(string message);
        public void LogError(string message);
        public void LogInfo(string message);
        public void LogWarn(string message);
    }
}

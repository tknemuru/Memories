using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
    /// <summary>
    /// 構築機能を提供します。
    /// </summary>
    public interface IBuildable
    {
        /// <summary>
        /// 構築します。
        /// </summary>
        /// <returns>構築した文字列</returns>
        string Build();
    }
}

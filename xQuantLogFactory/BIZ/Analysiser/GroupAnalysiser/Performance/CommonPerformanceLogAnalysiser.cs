using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser.Performance
{
    public class CommonPerformanceLogAnalysiser : GroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPerformanceLogAnalysiser"/> class.
        /// </summary>
        public CommonPerformanceLogAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPerformanceLogAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CommonPerformanceLogAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            throw new NotImplementedException();
        }
    }
}

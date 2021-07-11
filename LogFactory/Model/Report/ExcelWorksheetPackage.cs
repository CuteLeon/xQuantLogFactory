using System;

using OfficeOpenXml;

namespace LogFactory.Model.Report
{
    /// <summary>
    /// Excel 导出表数据包
    /// </summary>
    public class ExcelWorksheetPackage : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWorksheetPackage"/> class.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowID"></param>
        /// <param name="excelRange"></param>
        public ExcelWorksheetPackage(ExcelWorksheet worksheet, int rowID, ExcelRange excelRange)
        {
            this.Worksheet = worksheet;
            this.RowID = rowID;
            this.ExcelRange = excelRange;
        }

        /// <summary>
        /// Gets or sets 数据表
        /// </summary>
        public ExcelWorksheet Worksheet { get; protected set; }

        /// <summary>
        /// Gets or sets 行号
        /// </summary>
        public int RowID { get; set; }

        /// <summary>
        /// Gets or sets 数据区域
        /// </summary>
        public ExcelRange ExcelRange { get; protected set; }

        /// <summary>
        /// 行号自增
        /// </summary>
        public void RowIDIncrease() => this.RowID++;

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // 不要释放 Worksheet 对象，否则无法保存
                    // this.Worksheet?.Dispose();
                    this.ExcelRange?.Dispose();
                }

                this.Worksheet = null;
                this.ExcelRange = null;

                this.disposedValue = true;
            }
        }

        ~ExcelWorksheetPackage()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);

            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

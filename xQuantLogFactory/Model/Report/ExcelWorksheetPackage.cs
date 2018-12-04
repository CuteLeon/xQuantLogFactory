using System;

using OfficeOpenXml;

namespace xQuantLogFactory.Model.Report
{
    /// <summary>
    /// Excel 导出表数据包
    /// </summary>
    public class ExcelWorksheetPackage : IDisposable
    {
        public ExcelWorksheetPackage(ExcelWorksheet worksheet, int rowNumber, ExcelRange excelRange)
        {
            this.Worksheet = worksheet;
            this.RowNumber = rowNumber;
            this.ExcelRange = excelRange;
        }

        /// <summary>
        /// Gets or sets 数据表
        /// </summary>
        public ExcelWorksheet Worksheet { get; protected set; }

        /// <summary>
        /// Gets or sets 行号
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets or sets 数据区域
        /// </summary>
        public ExcelRange ExcelRange { get; protected set; }

        /// <summary>
        /// 行号自增
        /// </summary>
        public void RowNumberIncrease() => this.RowNumber++;

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Worksheet?.Dispose();
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

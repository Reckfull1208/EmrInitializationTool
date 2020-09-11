using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Librarys.Model
{
    public class T_Base_EMRTemplateClassify
    {
        #region 简单属性

        /// <summary>
        /// GUID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Hos_ID
        /// </summary>
        public string Hos_ID { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// PYM
        /// </summary>
        public string PYM { get; set; }

        /// <summary>
        /// SortNumber
        /// </summary>
        public int SortNumber { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// Creator_ID
        /// </summary>
        public string Creator_ID { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// Operator_ID
        /// </summary>
        public string Operator_ID { get; set; }

        /// <summary>
        /// OperateDate
        /// </summary>
        public System.DateTime OperateDate { get; set; }

        /// <summary>
        /// RowVersion
        /// </summary>
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// FatherID
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// IsUseFatherQC
        /// </summary>
        public bool IsUseFatherQC { get; set; }

        /// <summary>
        /// Enable
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// IsUse
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// ReadOnly
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// IsDelete
        /// </summary>
        public bool IsDelete { get; set; }

        #endregion
    }
}

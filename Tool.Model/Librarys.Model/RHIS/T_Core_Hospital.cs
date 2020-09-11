using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Librarys.Model
{
    public class T_Core_Hospital
    {
        #region 简单属性

        /// <summary>
        /// GUID
        /// </summary> 
        public string GUID { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary> 
        public string Father_ID { get; set; }

        /// <summary>
        /// 院区名称
        /// </summary> 
        public string Hos_Name { get; set; }

        /// <summary>
        /// 拼音码
        /// </summary> 
        public string PYM { get; set; }

        /// <summary>
        /// 院区编号
        /// </summary> 
        public string Hos_Code { get; set; }

        /// <summary>
        /// 是否根目录
        /// </summary> 
        public bool IsRoot { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary> 
        public bool Enable { get; set; }

        /// <summary>
        /// 机构等级
        /// </summary> 
        public string HospitalGrade { get; set; }

        /// <summary>
        /// 院区成立时间
        /// </summary> 
        public string BulidYear { get; set; }

        /// <summary>
        /// 院区地址
        /// </summary> 
        public string Hos_Address { get; set; }

        /// <summary>
        /// 邮编编码
        /// </summary> 
        public string PostCode { get; set; }

        /// <summary>
        /// 院区网址
        /// </summary> 
        public string Hos_Website { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary> 
        public string ContactNumber { get; set; }

        /// <summary>
        /// 法人代表
        /// </summary> 
        public string LegalRepresentative { get; set; }

        /// <summary>
        /// 组织机构代码
        /// </summary> 
        public string OrgCode { get; set; }

        /// <summary>
        /// 分类管理代码
        /// </summary> 
        public string OrgMagagerCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary> 
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary> 
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary> 
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// Synopsis
        /// </summary> 
        public string Synopsis { get; set; }

        /// <summary>
        /// WXSynopsis
        /// </summary> 
        public string WXSynopsis { get; set; }

        /// <summary>
        /// legalPerCertNO
        /// </summary> 
        public string legalPerCertNO { get; set; }

        /// <summary>
        /// LicenseNo
        /// </summary> 
        public string LicenseNo { get; set; }

        /// <summary>
        /// OrgNo
        /// </summary> 
        public string OrgNo { get; set; } 

        /// <summary>
        /// CreditCode
        /// </summary> 
        public string CreditCode { get; set; }

        #endregion
    }
}

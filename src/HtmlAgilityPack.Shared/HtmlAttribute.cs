// Description: Html Agility Pack - HTML Parsers, selectors, traversors, manupulators.
// Website & Documentation: http://html-agility-pack.net
// Forum & Issues: https://github.com/zzzprojects/html-agility-pack
// License: https://github.com/zzzprojects/html-agility-pack/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright ?ZZZ Projects Inc. 2014 - 2017. All rights reserved.

#region

using System;
using System.Diagnostics;

#endregion

// ReSharper disable InconsistentNaming

namespace HtmlAgilityPack
{
    /// <summary>
    /// Represents an HTML attribute.
    /// 表示一个HTML属性
    /// </summary>
    [DebuggerDisplay("Name: {OriginalName}, Value: {Value}")]
    public class HtmlAttribute : IComparable
    {
        #region Fields

        private int _line;
        internal int _lineposition;
        internal string _name;
        internal int _namelength;
        internal int _namestartindex;
        internal HtmlDocument _ownerdocument; // attribute can exists without a node
        internal HtmlNode _ownernode;
        private AttributeValueQuote _quoteType = AttributeValueQuote.DoubleQuote;
        internal int _streamposition;
        internal string _value;
        internal int _valuelength;
        internal int _valuestartindex; 
        internal bool _isFromParse;
        internal bool _hasEqual;
        private bool? _localUseOriginalName;

        #endregion

        #region Constructors

        internal HtmlAttribute(HtmlDocument ownerdocument)
        {
            _ownerdocument = ownerdocument;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the line number of this attribute in the document.
        /// 获取文档中此属性的行号。
        /// </summary>
        public int Line
        {
            get { return _line; }
            internal set { _line = value; }
        }

        /// <summary>
        /// Gets the column number of this attribute in the document.
        /// 获取文档中此属性的列号。
        /// </summary>
        public int LinePosition
        {
            get { return _lineposition; }
        }

        /// <summary>
        /// Gets the stream position of the value of this attribute in the document, relative to the start of the document.
        /// 获取此属性值在文档中的相对于文档开头的流位置。
        /// </summary>
        public int ValueStartIndex
        {
            get { return _valuestartindex; }
        }

        /// <summary>
        /// Gets the length of the value.
        /// 获取值的长度
        /// </summary>
        public int ValueLength
        {
            get { return _valuelength; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute should use the original name.
        /// 获取或设置一个值，该值指示属性是否应使用原始名称。
        /// </summary>
        /// <value>
        /// True if the attribute should use the original name, false if not.
        /// 如果属性应该使用原始名称则为True，否则为false。
        /// </value>
        public bool UseOriginalName
        {
            get
            {
                var useOriginalName = false;
                if (this._localUseOriginalName.HasValue)
				{
                    useOriginalName = this._localUseOriginalName.Value;
                }
                else if (this.OwnerDocument != null)
				{
                    useOriginalName = this.OwnerDocument.OptionDefaultUseOriginalName;
                }

                return useOriginalName;
            }
            set
            {
                this._localUseOriginalName = value;
            }
        }

        /// <summary>
        /// Gets the qualified name of the attribute.
        /// 获取属性的限定名。
        /// </summary>
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _ownerdocument.Text.Substring(_namestartindex, _namelength);
                }

	            return UseOriginalName ? _name : _name.ToLowerInvariant();
			}
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _name = value;
                if (_ownernode != null)
                {
                    _ownernode.SetChanged();
                }
            }
        }

        /// <summary>
        /// Name of attribute with original case
        /// 属性名与原始情况
        /// </summary>
        public string OriginalName
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the HTML document to which this attribute belongs.
        /// 获取此属性所属的HTML文档。
        /// </summary>
        public HtmlDocument OwnerDocument
        {
            get { return _ownerdocument; }
        }

        /// <summary>
        /// Gets the HTML node to which this attribute belongs.
        /// 获取此属性所属的HTML节点。
        /// </summary>
        public HtmlNode OwnerNode
        {
            get { return _ownernode; }
        }

        /// <summary>
        /// Specifies what type of quote the data should be wrapped in
        /// 指定应该将数据封装在哪种类型的引号中
        /// </summary>
        public AttributeValueQuote QuoteType
        {
            get { return _quoteType; }
            set { _quoteType = value; }
        }

        /// <summary>
        /// Specifies what type of quote the data should be wrapped in (internal to keep backward compatibility)
        /// 指定数据应该被包装在什么类型的引号中(内部以保持向后兼容性)
        /// </summary>
        internal AttributeValueQuote InternalQuoteType { get; set; }

        /// <summary>
        /// Gets the stream position of this attribute in the document, relative to the start of the document.
        /// 获取此属性在文档中相对于文档开头的流位置。
        /// </summary>
        public int StreamPosition
        {
            get { return _streamposition; }
        }

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// 获取或设置属性的值。
        /// </summary>
        public string Value
        {
            get
            {
                // A null value has been provided, the attribute should be considered as "hidden"
                //如果提供了空值，属性应该被认为是“隐藏的”
                if (_value == null && _ownerdocument.Text == null && _valuestartindex == 0 && _valuelength == 0)
                {
                    return null;
                }

                if (_value == null)
                {
                    _value = _ownerdocument.Text.Substring(_valuestartindex, _valuelength);

                    if (!_ownerdocument.BackwardCompatibility)
                    {
                        _value = HtmlEntity.DeEntitize(_value);
                    }
                }

                return _value;
            }
            set
            {
                _value = value;

                if (_ownernode != null)
                {
                    _ownernode.SetChanged();
                }
            }
        }

        /// <summary>
        /// Gets the DeEntitized value of the attribute.
        /// 获取属性的反实体化值。
        /// </summary>
        public string DeEntitizeValue
        {
            get { return HtmlEntity.DeEntitize(Value); }
        }

        internal string XmlName
        {
            get { return HtmlDocument.GetXmlName(Name, true, OwnerDocument.OptionPreserveXmlNamespaces); }
        }

        internal string XmlValue
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets a valid XPath string that points to this Attribute
        /// 获取指向此属性的有效XPath字符串
        /// </summary>
        public string XPath
        {
            get
            {
                string basePath = (OwnerNode == null) ? "/" : OwnerNode.XPath + "/";
                return basePath + GetRelativeXpath();
            }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another attribute. Comparison is based on attributes' name.
        /// 比较当前实例与另一个属性。比较基于属性的名称。
        /// </summary>
        /// <param name="obj">An attribute to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the names comparison.
        /// 32位有符号整数，表示名称比较的相对顺序。
        /// </returns>
        public int CompareTo(object obj)
        {
            HtmlAttribute att = obj as HtmlAttribute;
            if (att == null)
            {
                throw new ArgumentException("obj");
            }

            return Name.CompareTo(att.Name);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a duplicate of this attribute.
        /// 创建此属性的副本。
        /// </summary>
        /// <returns>The cloned attribute.</returns>
        public HtmlAttribute Clone()
        {
            HtmlAttribute att = new HtmlAttribute(_ownerdocument);
            att.Name = OriginalName;
            att.Value = Value;
            att.QuoteType = QuoteType;
            att.InternalQuoteType = InternalQuoteType;

            att._isFromParse = _isFromParse;
            att._hasEqual = _hasEqual;
            return att;
        }

        /// <summary>
        /// Removes this attribute from it's parents collection
        /// 从其父集合中移除此属性
        /// </summary>
        public void Remove()
        {
            _ownernode.Attributes.Remove(this);
        }

        #endregion

        #region Private Methods

        private string GetRelativeXpath()
        {
            if (OwnerNode == null)
                return Name;

            int i = 1;
            foreach (HtmlAttribute node in OwnerNode.Attributes)
            {
                if (node.Name != Name) continue;

                if (node == this)
                    break;

                i++;
            }

            return "@" + Name + "[" + i + "]";
        }

        #endregion
    }

    /// <summary>
    /// An Enum representing different types of Quotes used for surrounding attribute values
    /// 一个Enum，表示用于包围属性值的不同类型的引号
    /// </summary>
    public enum AttributeValueQuote
    {
        /// <summary>
        /// A single quote mark '
        /// 单引号
        /// </summary>
        SingleQuote,

        /// <summary>
        /// A double quote mark "
        /// 双引号
        /// </summary>
        DoubleQuote,

        /// <summary>
        /// No quote mark
        /// 无引号
        /// </summary>
        None,

        
        /// <summary>
        /// Without the value such as '&lt;span readonly&gt;'
        /// 单值
        /// </summary>
        WithoutValue,

        /// <summary>
        /// The initial value (current value)
        /// 初始值
        /// </summary>
        Initial
    }
}
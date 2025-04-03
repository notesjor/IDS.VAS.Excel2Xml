using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.MAP.Toolbox.Model.Struktur
{

  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class overview
  {

    private pattern[] patternField;

    private family[] familyField;

    private string labelField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("pattern")]
    public pattern[] pattern
    {
      get
      {
        return this.patternField;
      }
      set
      {
        this.patternField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("family")]
    public family[] family
    {
      get
      {
        return this.familyField;
      }
      set
      {
        this.familyField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string label
    {
      get
      {
        return this.labelField;
      }
      set
      {
        this.labelField = value;
      }
    }
  }
}

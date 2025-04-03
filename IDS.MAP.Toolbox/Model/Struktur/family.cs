namespace IDS.MAP.Toolbox.Model.Struktur
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class family
  {

    private pattern[] patternField;

    private string idField;

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
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string id
    {
      get
      {
        return this.idField;
      }
      set
      {
        this.idField = value;
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
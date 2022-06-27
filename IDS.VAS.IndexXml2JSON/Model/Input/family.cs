/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class family {
    
  private object[] itemsField;
    
  private string idField;
    
  private string labelField;
    
  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("family", typeof(family))]
  [System.Xml.Serialization.XmlElementAttribute("pattern", typeof(pattern))]
  public object[] Items {
    get {
      return this.itemsField;
    }
    set {
      this.itemsField = value;
    }
  }
    
  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
  public string id {
    get {
      return this.idField;
    }
    set {
      this.idField = value;
    }
  }
    
  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
  public string label {
    get {
      return this.labelField;
    }
    set {
      this.labelField = value;
    }
  }
}
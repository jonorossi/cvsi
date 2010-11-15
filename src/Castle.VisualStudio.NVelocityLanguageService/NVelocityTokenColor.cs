namespace Castle.VisualStudio.NVelocityLanguageService
{
    public enum NVelocityTokenColor
    {
        NVText = 1,
        NVKeyword,
        NVComment,
        NVIdentifier,
        NVString,
        NVNumber,
        NVDirective,
        NVOperator,
        NVBracket,
        NVDictionaryDelimiter,
        NVDictionaryKey,
        NVDictionaryEquals,

        XmlText,
        XmlComment,
        XmlTagName,
        XmlAttributeName,
        XmlAttributeValue,
        XmlTagDelimiter,
        XmlOperator,
        XmlEntity,
        XmlCDataSection//,
        //XmlProcessingInstruction
    }
}
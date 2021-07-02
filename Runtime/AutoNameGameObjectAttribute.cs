using System;

public class AutoNameGameObjectAttribute : Attribute
{
    #region Public Fields

    public readonly string name;

    #endregion

    #region Constructors

    public AutoNameGameObjectAttribute(string name = default)
    {
        this.name = name;
    }

    #endregion
}
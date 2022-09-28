﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.IO;

[Serializable]
public enum Unit
{
    ml,
    units,
    l
}

[Serializable]
public enum Package
{
    box,
    flask,
    bottle,
    baxter
}
[Serializable]
public class Medicine : SearchResult { //inheritance added by stefan
    [XmlAttribute]
    public int mID;
    public string mName;
    public int mQuantity;
    public Unit mUnit;
    public Package mPackage;
    public string mPointsOfAttention;//holds points of attention separated by # as a splitter

	public Medicine()
    {
        mName = "";
        mID = 0;
        mQuantity = 0;
        mUnit = Unit.ml;
        mPackage = Package.box;
        mPointsOfAttention = "";
    }

    //added by stefan:

    public string Name
    {
        get{
            return mName;
        }
    }

    public override string ToResult()
    {
        return string.Format("<b>Name:</b> {0}\n<b>Package:</b> {1}\n<b>Quantity:</b> {2}\t<b>Unit:</b> {3}\n<b>Points of attention:</b>\n{4}", mName, mPackage.ToString(), mQuantity, mUnit, String.Join("&bull;",mPointsOfAttention.Split('#')));
    }

    public Medicine CleanUpName() //if a hashtag in name (e.G aspirine#gray) it will return the same medicine with a normal name
    {
        Medicine newMed = new Medicine();
        newMed.mID = this.mID;
        newMed.mName = this.mName.Split('#')[0];
        newMed.mQuantity = this.mQuantity;
        newMed.mUnit = this.mUnit;
        newMed.mPackage = this.mPackage;
        newMed.mPointsOfAttention = this.mPointsOfAttention;

        return newMed;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum LabelType
{
    anesthesia_equipment = 0,
    operating_table = 1,
    instrument_table = 2,
    secondary_table = 3,
    instrument = 4,
    otherobject = 5,
    Patient = 9,
    human_0 = 10,
    human_1 = 11,
    human_2 = 12,
    human_3 = 13,
    human_4 = 14,
    human_5 = 15,
    human_6 = 16,
    human_7 = 17
};


[Serializable]
public enum RelationType
{
    Close_To = 0,
    Touching = 1,
    Operating = 2,
    Other = 3
};

public class Configurations
{
    public static readonly Dictionary<string, string> NameMapping = new Dictionary<string, string>()
    {
        { "human_0", "Human 0" },
        { "human_1", "Human 1" },
        { "human_2", "Human 2" },
        { "human_3", "Human 3" },
        { "human_4", "Human 4" },
        { "human_5", "Human 5" },
        { "human_6", "Human 6" },
        { "secondary_table", "Second Table" },
        { "instrument_table", "Instrument Table" },
        { "anesthesia_equipment", "Anesthesia Equipment" },
        { "operating_table", "Operating Table" },
        { "Patient", "Patient" },
        { "instrument", "Instrument" }
    };

    public static readonly Dictionary<string, string> reverseMapping = new Dictionary<string, string>()
    {
        { "Human 0", "human_0" },
        { "Human 1", "human_1" },
        { "Human 2", "human_2" },
        { "Human 3", "human_3" },
        { "Human 4", "human_4" },
        { "Human 5", "human_5" },
        { "Human 6", "human_6" },
        { "Second Table", "secondary_table" },
        { "Instrument Table", "instrument_table" },
        { "Anesthesia Equipment", "anesthesia_equipment" },
        { "Operating Table", "operating_table" },
        { "Patient", "Patient" },
        { "Instrument", "instrument" }
    };


    public static readonly Dictionary<string, LabelType> LabelMapping = new Dictionary<string, LabelType>()
    {
        { "Human 0", LabelType.human_0 },
        { "Human 1", LabelType.human_1 },
        { "Human 2", LabelType.human_2 },
        { "Human 3", LabelType.human_3 },
        { "Human 4", LabelType.human_4 },
        { "Human 5", LabelType.human_5 },
        { "Human 6", LabelType.human_6 },
        { "Second Table", LabelType.secondary_table },
        { "Instrument Table", LabelType.instrument_table },
        { "Anesthesia Equipment", LabelType.anesthesia_equipment },
        { "Operating Table", LabelType.operating_table },
        { "Patient", LabelType.Patient },
        { "Instrument", LabelType.instrument }
    };
    public static Color AssignColor(LabelType type)
    {
        Color _BoxColor;

        switch (type)
        {
            case LabelType.anesthesia_equipment:
                _BoxColor = new Color(0.96f, 0.576f, 0.65f, 1f);
                break;
            case LabelType.operating_table:
                _BoxColor = new Color(0.2f, 0.83f, 0.72f, 1f);
                break;
            case LabelType.instrument_table:
                _BoxColor = new Color(0.93f, 0.65f, 0.93f, 1f);
                break;
            case LabelType.secondary_table:
                _BoxColor = new Color(0.90f, 0.30f, 0.63f, 1f);
                break;
            case LabelType.instrument:
                _BoxColor = new Color(1.0f, 0.811f, 0.129f, 1f);
                break;
            case LabelType.otherobject:
                _BoxColor = new Color(0.61f, 0.48f, 0.04f, 1f);
                break;
            case LabelType.Patient:
                _BoxColor = new Color(0, 1f, 0f, 1f);
                break;
            case LabelType.human_0:
                _BoxColor = new Color(1, 0f, 0f, 1f);
                break;
            case LabelType.human_1:
                _BoxColor = new Color(0.9f, 0f, 0f, 1f);
                break;
            case LabelType.human_2:
                _BoxColor = new Color(0.85f, 0f, 0f, 1f);
                break;
            case LabelType.human_3:
                _BoxColor = new Color(0.8f, 0f, 0f, 1f);
                break;
            case LabelType.human_4:
                _BoxColor = new Color(0.75f, 0f, 0f, 1f);
                break;
            case LabelType.human_5:
                _BoxColor = new Color(0.7f, 0f, 0f, 1f);
                break;
            case LabelType.human_6:
                _BoxColor = new Color(0.65f, 0f, 0f, 1f);
                break;
            case LabelType.human_7:
                _BoxColor = new Color(0.6f, 0f, 0f, 1f);
                break;
            default:
                _BoxColor = new Color(1, 0f, 0f, 1f);
                break;
        }
        return _BoxColor;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        var vect = (Vector3)obj;
        info.AddValue("x", vect.x);
        info.AddValue("y", vect.y);
        info.AddValue("z", vect.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        var vect = (Vector3)obj;
        vect.x = (float)info.GetValue("x", typeof(float));
        vect.y = (float)info.GetValue("y", typeof(float));
        vect.z = (float)info.GetValue("z", typeof(float));
        obj = vect;
        return obj;
    }
}

public class QuaterionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        var quaterion = (Quaternion)obj;
        info.AddValue("x", quaterion.x);
        info.AddValue("y", quaterion.y);
        info.AddValue("z", quaterion.z);
        info.AddValue("w", quaterion.w);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        var quaterion = (Quaternion)obj;
        quaterion.x = (float)info.GetValue("x", typeof(float));
        quaterion.y = (float)info.GetValue("y", typeof(float));
        quaterion.z = (float)info.GetValue("z", typeof(float));
        quaterion.w = (float)info.GetValue("w", typeof(float));
        obj = quaterion;
        return obj;
    }
}

public class SerializationManager : MonoBehaviour
{
    public static bool Save(string saveName, object saveData)
    {
        IFormatter formatter = GetBinaryFormatter();

        string savePath = $@"{Application.persistentDataPath}/kesller_syndrome/{saveName}.save";

        if (!Directory.Exists($@"{Application.persistentDataPath}/kesller_syndrome"))
            Directory.CreateDirectory($@"{Application.persistentDataPath}/kesller_syndrome");

        using (FileStream file = File.Create(savePath))
        {
            formatter.Serialize(file, saveData);
        }        

        return true;
    }

    public static object Load(string saveName)
    {
        string savePath = $@"{Application.persistentDataPath}/kesller_syndrome/{saveName}.save";
        if (!File.Exists(savePath))
            return null;

        try
        {
            IFormatter formatter = GetBinaryFormatter();
            using (FileStream file = File.Open(savePath, FileMode.Open))
            {
                return formatter.Deserialize(file);
            }
        }
        catch
        {
            Debug.LogError($"Failed to read data from file {savePath}");
            return null;
        }
    }

    public static IFormatter GetBinaryFormatter()
    {
        var formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();
        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaterionSerializationSurrogate());

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}

﻿using Abstractions;
using Microsoft.FamilyShowLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Diagram.Logic
{

          [XmlRoot("Family")]
  //[XmlInclude(typeof(ParentRelationship))]
  //[XmlInclude(typeof(ChildRelationship))]
  //[XmlInclude(typeof(SpouseRelationship))]
  //[XmlInclude(typeof(SiblingRelationship))]
  public class People
  {
    #region fields and constants

    // The constants specific to this class
    private static class Const
    {
      public const string DataFileName = "default.family";
    }

    // Fields
    private PeopleCollection<INode> peopleCollection;

    // The current person's Id will be serialized instead of the current person object to avoid
    // circular references during Xml Serialization. When family data is loaded, the corresponding
    // person object will be assigned to the current property (please see app.xaml.cs).
    // The currentPersonId is set in the Save method of this class.
    private string currentPersonId;

    // Store the person's name with the Id to make the xml file more readable.
    // The currentPersonName is set in the Save method of this class.
    private string currentPersonName;

    // The fully qualified path and filename for the family file.
    private string fullyQualifiedFilename;

    // Version of the file. This is not used at this time, but allows a future
    // version of the application to handle previous file formats.
    private string fileVersion = "1.0";

    private string OPCContentFileName = "content.xml";

    #endregion

    #region Properties

    /// <summary>
    /// Collection of people.
    /// </summary>
    public PeopleCollection<INode> PeopleCollection
    {
      get { return peopleCollection; }
    }

    /// <summary>
    /// Id of currently selected person.
    /// </summary>
    [XmlAttribute(AttributeName = "Current")]
    public string CurrentPersonId
    {
      get { return currentPersonId; }
      set { currentPersonId = value; }
    }

    // Name of current selected person (included for readability in xml file).
    [XmlAttribute(AttributeName = "CurrentName")]
    public string CurrentPersonName
    {
      get { return currentPersonName; }
      set { currentPersonName = value; }
    }

    // Version of the file.
    [XmlAttribute(AttributeName = "FileVersion")]
    public string Version
    {
      get { return fileVersion; }
      set { fileVersion = value; }
    }

    //[XmlIgnore]
    //public static string ApplicationFolderPath
    //{
    //  get
    //  {
    //    return Path.Combine(
    //      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    //      App.ApplicationFolderName);
    //  }
    //}

    //[XmlIgnore]
    //public static string DefaultFullyQualifiedFilename
    //{
    //  get
    //  {
    //    // Absolute path to the application folder
    //    string appLocation = Path.Combine(
    //        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    //        App.ApplicationFolderName);

    //    // Create the directory if it doesn't exist
    //    if (!Directory.Exists(appLocation))
    //    {
    //      Directory.CreateDirectory(appLocation);
    //    }

    //    return Path.Combine(appLocation, Const.DataFileName);
    //  }
    //}

    /// <summary>
    /// Fully qualified filename (absolute pathname and filename) for the data file
    /// </summary>
    [XmlIgnore]
    public string FullyQualifiedFilename
    {
      get { return fullyQualifiedFilename; }

      set { fullyQualifiedFilename = value; }
    }

    #endregion

    public People()
    {
      peopleCollection = new PeopleCollection<INode>();
    }

    #region Loading and saving

    /// <summary>
    /// Persist the current list of people to disk.
    /// </summary>
    public void Save()
    {
      //// Return right away if nothing to save.
      //if (PeopleCollection == null || PeopleCollection.Count == 0)
      //{
      //  return;
      //}

      //// Set the current person id and name before serializing
      //CurrentPersonName = PeopleCollection.Current.FullName;
      //CurrentPersonId = PeopleCollection.Current.Id;

      //// Use the default path and filename if none was provided
      //if (string.IsNullOrEmpty(FullyQualifiedFilename))
      //{
      //  FullyQualifiedFilename = DefaultFullyQualifiedFilename;
      //}

      //// Setup temp folders for this family to be packaged into OPC later
      //string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ApplicationFolderName);
      //tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);

      //// Create the necessary directories
      //Directory.CreateDirectory(tempFolder);

      //// Create xml content file
      //XmlSerializer xml = new XmlSerializer(typeof(People));
      //using (Stream stream = new FileStream(Path.Combine(tempFolder, OPCContentFileName), FileMode.Create, FileAccess.Write, FileShare.None))
      //{
      //  xml.Serialize(stream, this);
      //}

      //// save to file package
      //OPCUtility.CreatePackage(FullyQualifiedFilename, tempFolder);

      //PeopleCollection.IsDirty = false;
    }

    /// <summary>
    /// Saves the list of people to disk using the specified filename and path
    /// </summary>
    /// <param name="FQFilename">Fully qualified path and filename of family tree file to save</param>
    public void Save(string FQFilename)
    {
      fullyQualifiedFilename = FQFilename;
      Save();
    }

    /// <summary>
    /// Load the list of people from Family.Show version 2.0 file format
    /// </summary>
  //  [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public void LoadVersion2()
    {
      //// Loading, clear existing nodes
      //PeopleCollection.Clear();

      //try
      //{
      //  // Use the default path and filename if none were provided
      //  if (string.IsNullOrEmpty(FullyQualifiedFilename))
      //  {
      //    FullyQualifiedFilename = DefaultFullyQualifiedFilename;
      //  }

      //  XmlSerializer xml = new XmlSerializer(typeof(People));
      //  using (Stream stream = new FileStream(FullyQualifiedFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
      //  {
      //    People people = (People)xml.Deserialize(stream);
      //    stream.Close();

      //    foreach (IId person in people.PeopleCollection)
      //    {
      //      PeopleCollection.Add(person);
      //    }

      //    // Setup temp folders for this family to be packaged into OPC later
      //    string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      //        App.ApplicationFolderName);
      //    tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);
      //    RecreateDirectory(tempFolder);

      //    string photoFolder = Path.Combine(tempFolder, Photo.Const.PhotosFolderName);
      //    RecreateDirectory(photoFolder);

      //    string storyFolder = Path.Combine(tempFolder, Story.Const.StoriesFolderName);
      //    RecreateDirectory(storyFolder);

      //    foreach (Person person in PeopleCollection)
      //    {
      //      // To avoid circular references when serializing family data to xml, only the person Id
      //      // is seralized to express relationships. When family data is loaded, the correct
      //      // person object is found using the person Id and assigned to the appropriate relationship.
      //      foreach (Relationship relationship in person.Relationships)
      //      {
      //        relationship.RelationTo = PeopleCollection.Find(relationship.PersonId);
      //      }

      //      // store the stories into temp directory to be packaged into OPC later
      //      foreach (Photo photo in person.Photos)
      //      {
      //        string photoOldPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), App.ApplicationFolderName), photo.RelativePath);
      //        if (File.Exists(photoOldPath))
      //        {
      //          string photoFile = Path.Combine(photoFolder, Path.GetFileName(photo.FullyQualifiedPath));

      //          // Remove spaces since they'll be packaged as %20, breaking relative paths that expect spaces
      //          photoFile = photoFile.Replace(" ", "");
      //          photo.RelativePath = photo.RelativePath.Replace(" ", "");
      //          File.Copy(photoOldPath, photoFile, true);
      //        }
      //      }

      //      // store the person's story into temp directory to be packaged into OPC later
      //      if (person.Story != null)
      //      {
      //        string storyOldPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), App.ApplicationFolderName), person.Story.RelativePath);
      //        if (File.Exists(storyOldPath))
      //        {
      //          string storyFile = Path.Combine(storyFolder, Path.GetFileName(person.Story.AbsolutePath));

      //          // Remove spaces since they'll be packaged as %20, breaking relative paths that expect spaces
      //          storyFile = ReplaceEncodedCharacters(storyFile);
      //          person.Story.RelativePath = ReplaceEncodedCharacters(person.Story.RelativePath);

      //          File.Copy(storyOldPath, storyFile, true);
      //        }
      //      }
      //    }

      //    // Set the current person in the list
      //    CurrentPersonId = people.CurrentPersonId;
      //    CurrentPersonName = people.CurrentPersonName;
      //    PeopleCollection.Current = PeopleCollection.Find(CurrentPersonId);
      //  }

      //  PeopleCollection.IsDirty = false;
      //  return;
      //}
      //catch (Exception)
      //{
      //  // Could not load the file. Handle all exceptions
      //  // the same, ignore and continue.
      //  fullyQualifiedFilename = string.Empty;
      //}
    }

    private static string ReplaceEncodedCharacters(string fileName)
    {
      fileName = fileName.Replace(" ", "");
      fileName = fileName.Replace("{", "");
      fileName = fileName.Replace("}", "");
      return fileName;
    }

    /// <summary>
    /// Delete to clear existing files and re-Create the necessary directories
    /// </summary>
    public static void RecreateDirectory(string folderToDelete)
    {
      try
      {
        if (Directory.Exists(folderToDelete))
        {
          Directory.Delete(folderToDelete, true);
        }

        Directory.CreateDirectory(folderToDelete);
      }
      catch
      {
        // ignore deletion errors
      }
    }

    /// <summary>
    /// Load the list of people from disk using the Open Package Convention format
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public void LoadOPC()
    {
      //// Loading, clear existing nodes
      //PeopleCollection.Clear();

      //try
      //{
      //  // Use the default path and filename if none were provided
      //  if (string.IsNullOrEmpty(FullyQualifiedFilename))
      //  {
      //    FullyQualifiedFilename = DefaultFullyQualifiedFilename;
      //  }

      //  string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ApplicationFolderName);

      //  tempFolder = Path.Combine(tempFolder, App.AppDataFolderName + @"\");

      //  OPCUtility.ExtractPackage(FullyQualifiedFilename, tempFolder);

      //  XmlSerializer xml = new XmlSerializer(typeof(People));
      //  using (Stream stream = new FileStream(tempFolder + OPCContentFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      //  {
      //    People people = (People)xml.Deserialize(stream);
      //    stream.Close();

      //    foreach (Person person in people.PeopleCollection)
      //    {
      //      PeopleCollection.Add(person);
      //    }

      //    // To avoid circular references when serializing family data to xml, only the person Id
      //    // is seralized to express relationships. When family data is loaded, the correct
      //    // person object is found using the person Id and assigned to the appropriate relationship.
      //    foreach (Person person in PeopleCollection)
      //    {
      //      foreach (Relationship relationqhip in person.Relationships)
      //      {
      //        relationqhip.RelationTo = PeopleCollection.Find(relationqhip.PersonId);
      //      }
      //    }

      //    // Set the current person in the list
      //    CurrentPersonId = people.CurrentPersonId;
      //    CurrentPersonName = people.CurrentPersonName;
      //    PeopleCollection.Current = PeopleCollection.Find(CurrentPersonId);
      //  }

      //  PeopleCollection.IsDirty = false;
      //  return;
      //}
      //catch
      //{
      //  // Could not load the file. Handle all exceptions
      //  // the same, ignore and continue.
      //  fullyQualifiedFilename = string.Empty;
      //}
    }

    #endregion
  }
    }


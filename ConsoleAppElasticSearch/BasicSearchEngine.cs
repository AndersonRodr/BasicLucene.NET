using Lucene.Net.Index;
using Lucene.Net.Store;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System;
using Lucene.Net.QueryParsers.Classic;

namespace ConsoleAppElasticSearch
{
    public  class BasicSearchEngine
    {
        public static IndexWriter Writer { get; set; }
        public static List<Person> Data { get; set; }
        private static RAMDirectory _directory;
        private static string _personGuidToBeUpdated;


        public static void Index()
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            _directory = new RAMDirectory();
            var config = new IndexWriterConfig(lv, a);
            Writer = new IndexWriter(_directory, config);
        }

        public static void GetData()
        {
            Data = new List<Person>(){
                new Person(Guid.NewGuid().ToString(),"Fred","George","Herb","A tall thin man."),
                new Person(Guid.NewGuid().ToString(),"Alice","Frey","Ally","A small thin woman."),
                new Person(Guid.NewGuid().ToString(),"Mia","Gray","Mia","A thin woman."),
                new Person(Guid.NewGuid().ToString(),"Mark","Russel","Mark","A fat man."),
                new Person(Guid.NewGuid().ToString(),"Bruce","Wayne","Bruce","A rich man."),
                new Person(Guid.NewGuid().ToString(),"Diana","Amazon","Diana","A young woman."),
                new Person(Guid.NewGuid().ToString(),"Peter","Parker","Pet","A super hero."),
                new Person(Guid.NewGuid().ToString(),"Frank","Ed","Stevens","A short fat man."),
                new Person(Guid.NewGuid().ToString(),"Alfred","Edward","Stewart","A medium average man."),
                new Person(Guid.NewGuid().ToString(),"Joe","Rand","Smith","A very tall large man."),
                new Person(Guid.NewGuid().ToString(),"Abigal","Elizabeth","Spear","A tall thin woman."),
                new Person(Guid.NewGuid().ToString(),"Michael","Rose","Garcia","A small average woman."),
                new Person(Guid.NewGuid().ToString(),"Deborah","Jordan","Davis","A tall large woman."),
                new Person(Guid.NewGuid().ToString(),"Bertha","Madison","Jones","A short fat woman."),
                new Person(Guid.NewGuid().ToString(),"Clint","Johnny","Williams","A very tiny boy."),
                new Person(Guid.NewGuid().ToString(),"Susan","Michele","Brown","A very tiny girl."),
                new Person(Guid.NewGuid().ToString(),"John","Snow","Jo","A warrior and brave man."),
                new Person(Guid.NewGuid().ToString(),"Sue","Storm","Sue","A gorgeous blonde woman."),
                new Person(Guid.NewGuid().ToString(),"Daenerys","Targaryen","Danny","A blonde brave woman."),
                new Person(Guid.NewGuid().ToString(),"Clarck","Kent","Clark","A cryptonian man and super hero"),
                new Person(Guid.NewGuid().ToString(),"Bruce","Banner","Stewart","A medium average man."),
                new Person(Guid.NewGuid().ToString(),"Lionel","Messi","Leo","A short man. A great soccer player."),
                new Person(Guid.NewGuid().ToString(),"Cristiano","Ronaldo","CR7","A great soccer player."),
            };

            var guidField = new StringField("GUID", "", Field.Store.YES);
            var fNameField = new TextField("FirstName", "", Field.Store.YES);
            var mNameField = new TextField("MiddleName", "", Field.Store.YES);
            var lNameField = new TextField("LastName", "", Field.Store.YES);
            var descriptionField = new TextField("Description", "", Field.Store.YES);

            var d = new Document()
            {
                guidField,
                fNameField,
                mNameField,
                lNameField,
                descriptionField
            };

            foreach (Person person in Data)
            {
                guidField.SetStringValue(person.GUID);
                fNameField.SetStringValue(person.FirstName);
                mNameField.SetStringValue(person.MiddleName);
                lNameField.SetStringValue(person.LastName);
                descriptionField.SetStringValue(person.Description);

                Writer.AddDocument(d);
            }

            Writer.Commit();
        }

        public static void AddToIndex()
        {
            _personGuidToBeUpdated = Guid.NewGuid().ToString();

            var d = new Document()
            {
                new StringField("GUID", _personGuidToBeUpdated, Field.Store.YES),
                new TextField("FirstName", "AddedFirstName", Field.Store.YES),
                new TextField("MiddleName", "AddedMiddleName", Field.Store.YES),
                new TextField("LastName", "AddedLastName", Field.Store.YES),
                new TextField("Description", "Added Description", Field.Store.YES)
            };

            Writer.AddDocument(d);
            Writer.Commit();
        }

        public static void ChangeInIndex()
        {
            var d = new Document()
            {
                new StringField("GUID", _personGuidToBeUpdated, Field.Store.YES),
                new TextField("FirstName", "UpdateFirstName", Field.Store.YES),
                new TextField("MiddleName", "UpdatedMiddleName", Field.Store.YES),
                new TextField("LastName", "UpdatedLastName", Field.Store.YES),
                new TextField("Description", "Updated Description", Field.Store.YES)
            };

            Writer.UpdateDocument(new Term("GUID", _personGuidToBeUpdated), d);
            Writer.Commit();
        }

        public static void DeleteFromIndex()
        {
            Writer.DeleteDocuments(new Term("GUID", _personGuidToBeUpdated));
            Writer.Commit();
        }

        public static void Dispose()
        {
            Writer.Dispose();
            _directory.Dispose();
        }

        public static List<string> Search(string input)
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            var dirReader = DirectoryReader.Open(_directory);
            var searcher = new IndexSearcher(dirReader);

            string[] fnames = { "GUID", "FirstName", "MiddleName", "LastName", "Age", "Description" };

            WildcardQuery query = new WildcardQuery(new Term("Description", input.Trim() + "*"));
            TopDocs resDocs = searcher.Search(query, 10);
            var results = new List<string>();

            for (int i = 0; i < resDocs.TotalHits; i++)
            {
                Document doctemp = searcher.Doc(resDocs.ScoreDocs[i].Doc);
                string guid = doctemp.Get("GUID");
                string firstname = doctemp.Get("FirstName");
                string middlename = doctemp.Get("MiddleName");
                string lastname = doctemp.Get("LastName");
                string description = doctemp.Get("Description");

                results.Add($"{guid} {firstname} {middlename} {lastname} {description}");
            }

            dirReader.Dispose();
            return results;
        }

    }
}

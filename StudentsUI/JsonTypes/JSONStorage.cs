using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;


namespace StudentsUI.JsonTypes
{
    
    internal class JSONStorage
    {
        private static readonly string DefaultFile = "C:/Users/SupaM/Documents/Magnus/Yinghua/people.json";
        private static readonly string WrittenFile = "./people.json";
        public static JSONStorage? Instance;
        public JsonNode RootNode;
        public Student[] students = new Student[0];
        public List<School> schools = new List<School>(0);
        public JSONStorage()
        {
            Instance = this;
        }
        public void Load(ListView list)
        { 
            
            for(int i = 0;i < list.Items.Count;i++)
            {
                Load(list.Items[i]);
            }
        }
        public void Load(ListViewItem item)
        {
            int id = int.Parse(item.SubItems[4].Text);
            for (int i = 0;i < students.Length;i++)
            {
                Student student = students[i];
                if(student.Id == id)
                {
                    student.FirstName = item.SubItems[0].Text;
                    student.LastName = item.SubItems[1].Text;
                    student.School = item.SubItems[2].Text;
                    student.Discord = item.SubItems[3].Text;
                    students[i] = student;
                    break;
                }
            }
        }
        public Student GetStudent(Student student,ListViewItem item)
        {
            student.FirstName = item.SubItems[0].Text;
            student.LastName = item.SubItems[1].Text;
            student.School = item.SubItems[2].Text;
            student.Discord = item.SubItems[3].Text;
            return student;
        }
        public void Read()
        {
            string data = File.ReadAllText(WrittenFile);
            RootNode = JsonNode.Parse(data);
            JsonObject root = RootNode.AsObject();
            JsonArray list = root["people"].AsArray();
            students = new Student[list.Count];
            schools = new List<School>(1024);
            for(int i = 0;i < students.Length;i++)
            {
                JsonNode? node = list[i];
                if (node == null) continue;
                Student student = GetStudent(node.AsObject());
                students[i] = student;
                School school;
                if (!TryGetSchool(student.School, out school))
                {
                    school = new School();
                    school.Name = student.School;
                }
                school.students.Add(student);
                student.school = school;

            }
        }
        public void Write()
        {
            JsonArray list = RootNode["people"].AsArray();
            list.Clear();
            foreach(Student student in students)
            {
                list.Add(GetStudent(student));
            }
            string data = RootNode.ToJsonString();
            File.WriteAllText(WrittenFile, data);
        }
        public bool TryGetSchool(string name,out School school)
        {
            for(int i = 0;i < schools.Count;i++)
            {
                if(schools[i].Name == name)
                {
                    school = schools[i];
                    return true;
                }
            }
            school = School.UnknownSchool;
            return false;
        }
        public Student GetStudent(JsonObject obj)
        {
            Student student = new Student();
            int? id = (int)obj["id"];
            string? firstname = (string?)obj["firstname"];
            string? lastname = (string?)obj["lastname"];
            string? school = (string?)obj["school"];
            string? discord = (string?)obj["discord"];
            JsonNode[]? _deadnames = obj["deadnames"]?.AsArray()?.ToArray();
            if (_deadnames == null) _deadnames = new JsonNode[0];
            string[] deadnames = Array.ConvertAll(_deadnames, item => (string)item);
            if(firstname == null)
            {
                student.FirstName = "Unknown";
            }
            else
            {
                student.FirstName = firstname;
            }
            if(lastname == null)
            {
                student.LastName = "Unknown";
            }
            else
            {
                student.LastName = lastname;
            }
            if(school == null)
            {
                student.School = "Unknown";
            }
            else
            {
                student.School = school;
            }
            if(id == null)
            {
                student.Id = -1;
            }
            else
            {
                student.Id = (int)id;
            }
            if(discord == null)
            {
                student.Discord = "Unknown";
            }
            else
            {
                student.Discord = discord;
            }
            if(deadnames == null)
            {
                student.DeadNames = new string[0];
            }
            else
            {
                student.DeadNames = deadnames;
            }
            return student;

        }
        public JsonObject GetStudent(Student student)
        {
            JsonObject obj = new JsonObject();
            obj.Add("firstname", student.FirstName);
            obj.Add("lastname", student.LastName);
            obj.Add("id", student.Id);
            obj.Add("school", student.School);
            obj.Add("discord", student.Discord);
            obj.Add("deadnames", StringsToJsonArray(student.DeadNames));
            return obj;
        }
        public JsonArray StringsToJsonArray(string[] strings)
        {
            JsonArray arr = new JsonArray();
            foreach(string s in strings)
            {
                arr.Add((JsonNode)s);
            }
            return arr;
        }
        public Student GetStudent(JsonElement element)
        {
            Student student = new Student();
            int id;
            JsonElement _id;
            if(element.TryGetProperty("id",out _id))
            {
                id = int.Parse(_id.GetString());
            }
            else
            {
                id = -1;
            }
            string firstname;
            JsonElement _firstname;
            if(element.TryGetProperty("firstname",out _firstname))
            {
                firstname = _firstname.GetString();
            }
            else
            {
                firstname = "Unknown";
            }
            string lastname;
            JsonElement _lastname;
            if(element.TryGetProperty("lastname",out _lastname))
            {
                lastname = _lastname.GetString();
            }
            else
            {
                lastname = "Unknown";
            }
            string school;
            JsonElement _school;
            if(element.TryGetProperty("school",out _school))
            {
                school = _school.GetString();
            }
            else
            {
                school = "Unknown";
                
            }
            string discord;
            JsonElement _discord;
            if(element.TryGetProperty("discord",out _discord))
            {
                discord = _discord.GetString();
            }
            else
            {
                discord = "Unknown";
            }
            string[]? deadnames = null;
            JsonElement _deadnames;
            if(element.TryGetProperty("deadnames",out _deadnames))
            {
                JsonElement[] names = _deadnames.EnumerateArray().ToArray();
                deadnames = new string[names.Length];
                for(int i = 0; i < deadnames.Length; i++)
                {
                    deadnames[i] = names[i].GetString();
                }
            }
            else
            {
                deadnames = new string[0];
            }
            student.FirstName = firstname;
            student.LastName = lastname;
            student.School = school;
            student.Id = id;
            student.DeadNames = deadnames;
            return student;
        }
    }
}

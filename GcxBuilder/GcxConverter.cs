using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace GcxEditor
{
    public class ProcedureElementConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IProcedureElement);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if(token.Type == JTokenType.Array)
            {
                var list = new List<IProcedureElement>();
                foreach(var item in token)
                {
                    list.Add(ConvertElement(item as JObject, serializer));
                }
                return list;
            }

            return ConvertElement(token as JObject, serializer);
        }

        private IProcedureElement ConvertElement(JObject jo, JsonSerializer serializer)
        {
            //JObject jo = JObject.Load(reader);
            if (jo == null)
                return null;
            string type = jo["Type"]?.ToString();

            IProcedureElement procedure = null;
            switch (type)
            {
                case "Expression":
                    var expression = JsonConvert.DeserializeObject<Expression>(jo.ToString());
                    procedure = expression;
                    break;
                case "Procedure":
                    var subprocedure = JsonConvert.DeserializeObject<Procedure>(jo.ToString());
                    procedure = subprocedure;
                    break;
                case "Invoke":
                    var invoke = JsonConvert.DeserializeObject<Invoke>(jo.ToString());
                    procedure = invoke;
                    break;
                case "GameCommand":
                    var command = JsonConvert.DeserializeObject<GameCommand>(jo.ToString());
                    procedure = command;
                    break;
                case "Msg":
                    var msg = JsonConvert.DeserializeObject<Msg>(jo.ToString());
                    procedure = msg;
                    break;
                case "Chara":
                    var chara = JsonConvert.DeserializeObject<Chara>(jo.ToString());
                    procedure = chara;
                    break;
                case "Trap":
                    var trap = JsonConvert.DeserializeObject<Trap>(jo.ToString());
                    procedure = trap;
                    break;
                case "Load":
                    var load = JsonConvert.DeserializeObject<Load>(jo.ToString());
                    procedure = load;
                    break;
                case "UnknownCommand":
                    var unknownCommand = JsonConvert.DeserializeObject<UnknownCommand>(jo.ToString());
                    procedure = unknownCommand;
                    break;
                case "Restart":
                    var restart = JsonConvert.DeserializeObject<Restart>(jo.ToString());
                    procedure = restart;
                    break;
                case "IfBlock":
                    var ifBlock = JsonConvert.DeserializeObject<IfBlock>(jo.ToString());
                    procedure = ifBlock;
                    break;
                case "SwitchBlock":
                    var switchBlock = JsonConvert.DeserializeObject<SwitchBlock>(jo.ToString());
                    procedure = switchBlock;
                    break;
                case "Print":
                    var print = JsonConvert.DeserializeObject<Print>(jo.ToString());
                    procedure = print;
                    break;
                case "Return":
                    var returnStatement = JsonConvert.DeserializeObject<Return>(jo.ToString());
                    procedure = returnStatement;
                    break;
                default:

                    break;
            }

            return procedure;
        }

        /*public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            JObject jo = JObject.FromObject(value, serializer);

            jo.WriteTo(writer);
        }*/

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class TermConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Term);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                var list = new List<Term?>();
                foreach (var item in token)
                {
                    list.Add(ConvertElement(item as JObject, serializer));
                }
                return list;
            }

            return ConvertElement(token as JObject, serializer);
        }

        private Term ConvertElement(JObject jo, JsonSerializer serializer)
        {
            //JObject jo = JObject.Load(reader);
            if (jo == null)
                return null;
            string type = jo["Type"]?.ToString();

            Term? procedure = null;
            switch (type)
            {
                case "Varbuf":
                    var varbuf = JsonConvert.DeserializeObject<Varbuf>(jo.ToString());
                    procedure = varbuf;
                    break;
                case "Constant":
                    var constant = JsonConvert.DeserializeObject<Constant>(jo.ToString());
                    procedure = constant;
                    break;
                case "Literal":
                    var literal = JsonConvert.DeserializeObject<Literal>(jo.ToString());
                    procedure = literal;
                    break;
                case "PassedArg":
                    var passedArg = JsonConvert.DeserializeObject<PassedArg>(jo.ToString());
                    procedure = passedArg;
                    break;
                case "VariableArray":
                    var varArray = JsonConvert.DeserializeObject<VariableArray>(jo.ToString());
                    procedure = varArray;
                    break;
                case "Linkvarbuf":
                    var linkvarbuf = JsonConvert.DeserializeObject<Linkvarbuf>(jo.ToString());
                    procedure = linkvarbuf;
                    break;
                case "Localvarbuf":
                    var localVarbuf = JsonConvert.DeserializeObject<Localvarbuf>(jo.ToString());
                    procedure = localVarbuf;
                    break;
                case "LocalVar":
                    var localVar = JsonConvert.DeserializeObject<LocalVar>(jo.ToString());
                    procedure = localVar;
                    break;
                case "Procedure":
                    var subProcedure = JsonConvert.DeserializeObject<Procedure>(jo.ToString());
                    procedure = subProcedure;
                    break;
                case "Expression":
                    var expression = JsonConvert.DeserializeObject<Expression>(jo.ToString()); //TODO: breaks on null Term1 >:C
                    procedure = expression;
                    break;
                default:

                    break;
            }

            return procedure;
        }

        /*public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            JObject jo = JObject.FromObject(value, serializer);

            jo.WriteTo(writer);
        }*/

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

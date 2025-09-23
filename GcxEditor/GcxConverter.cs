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

        private IProcedureElement ConvertElement(JObject jObject, JsonSerializer serializer)
        {
            if (jObject == null)
                return null;
            string type = jObject["Type"]?.ToString();

            IProcedureElement procedure = null;
            switch (type)
            {
                case "Expression":
                    var expression = JsonConvert.DeserializeObject<Expression>(jObject.ToString());
                    procedure = expression;
                    break;
                case "Procedure":
                    var subprocedure = JsonConvert.DeserializeObject<Procedure>(jObject.ToString());
                    procedure = subprocedure;
                    break;
                case "Invoke":
                    var invoke = JsonConvert.DeserializeObject<Invoke>(jObject.ToString());
                    procedure = invoke;
                    break;
                case "GameCommand":
                    var command = JsonConvert.DeserializeObject<GameCommand>(jObject.ToString());
                    procedure = command;
                    break;
                case "Msg":
                    var msg = JsonConvert.DeserializeObject<Msg>(jObject.ToString());
                    procedure = msg;
                    break;
                case "Chara":
                    var chara = JsonConvert.DeserializeObject<Chara>(jObject.ToString());
                    procedure = chara;
                    break;
                case "Trap":
                    var trap = JsonConvert.DeserializeObject<Trap>(jObject.ToString());
                    procedure = trap;
                    break;
                case "Load":
                    var load = JsonConvert.DeserializeObject<Load>(jObject.ToString());
                    procedure = load;
                    break;
                case "UnknownCommand":
                    var unknownCommand = JsonConvert.DeserializeObject<UnknownCommand>(jObject.ToString());
                    procedure = unknownCommand;
                    break;
                case "Restart":
                    var restart = JsonConvert.DeserializeObject<Restart>(jObject.ToString());
                    procedure = restart;
                    break;
                case "IfBlock":
                    var ifBlock = JsonConvert.DeserializeObject<IfBlock>(jObject.ToString());
                    procedure = ifBlock;
                    break;
                case "SwitchBlock":
                    var switchBlock = JsonConvert.DeserializeObject<SwitchBlock>(jObject.ToString());
                    procedure = switchBlock;
                    break;
                case "Print":
                    var print = JsonConvert.DeserializeObject<Print>(jObject.ToString());
                    procedure = print;
                    break;
                case "Return":
                    var returnStatement = JsonConvert.DeserializeObject<Return>(jObject.ToString());
                    procedure = returnStatement;
                    break;
                default:
                    throw new InvalidDataException("Invalid procedure element supplied in json");
            }

            return procedure;
        }

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

        private Term ConvertElement(JObject jObject, JsonSerializer serializer)
        {
            if (jObject == null)
                return null;
            string type = jObject["Type"]?.ToString();

            Term? procedure = null;
            switch (type)
            {
                case "Varbuf":
                    var varbuf = JsonConvert.DeserializeObject<Varbuf>(jObject.ToString());
                    procedure = varbuf;
                    break;
                case "Constant":
                    var constant = JsonConvert.DeserializeObject<Constant>(jObject.ToString());
                    procedure = constant;
                    break;
                case "Literal":
                    var literal = JsonConvert.DeserializeObject<Literal>(jObject.ToString());
                    procedure = literal;
                    break;
                case "PassedArg":
                    var passedArg = JsonConvert.DeserializeObject<PassedArg>(jObject.ToString());
                    procedure = passedArg;
                    break;
                case "VariableArray":
                    var varArray = JsonConvert.DeserializeObject<VariableArray>(jObject.ToString());
                    procedure = varArray;
                    break;
                case "Linkvarbuf":
                    var linkvarbuf = JsonConvert.DeserializeObject<Linkvarbuf>(jObject.ToString());
                    procedure = linkvarbuf;
                    break;
                case "Localvarbuf":
                    var localVarbuf = JsonConvert.DeserializeObject<Localvarbuf>(jObject.ToString());
                    procedure = localVarbuf;
                    break;
                case "LocalVar":
                    var localVar = JsonConvert.DeserializeObject<LocalVar>(jObject.ToString());
                    procedure = localVar;
                    break;
                case "Procedure":
                    var subProcedure = JsonConvert.DeserializeObject<Procedure>(jObject.ToString());
                    procedure = subProcedure;
                    break;
                case "Expression":
                    var expression = JsonConvert.DeserializeObject<Expression>(jObject.ToString()); //TODO: breaks on null Term1 >:C (fixed, i think?)
                    procedure = expression;
                    break;
                default:
                    throw new InvalidDataException("Invalid term supplied in json");
            }

            return procedure;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

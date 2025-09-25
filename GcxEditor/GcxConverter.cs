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
                    list.Add(ConvertElement((item as JObject)!, serializer));
                }
                return list;
            }

            return ConvertElement((token as JObject)!, serializer);
        }

        private static IProcedureElement ConvertElement(JObject jObject, JsonSerializer serializer)
        {
            if (jObject == null)
                return null;
            string type = jObject["Type"]!.ToString();

            IProcedureElement procedure = type switch
            {
                "Expression" => JsonConvert.DeserializeObject<Expression>(jObject.ToString())!,
                "Procedure" => JsonConvert.DeserializeObject<Procedure>(jObject.ToString())!,
                "Invoke" => JsonConvert.DeserializeObject<Invoke>(jObject.ToString())!,
                "GameCommand" => JsonConvert.DeserializeObject<GameCommand>(jObject.ToString())!,
                "Msg" => JsonConvert.DeserializeObject<Msg>(jObject.ToString())!,
                "Chara" => JsonConvert.DeserializeObject<Chara>(jObject.ToString())!,
                "Trap" => JsonConvert.DeserializeObject<Trap>(jObject.ToString())!,
                "Load" => JsonConvert.DeserializeObject<Load>(jObject.ToString())!,
                "UnknownCommand" => JsonConvert.DeserializeObject<UnknownCommand>(jObject.ToString())!,
                "Restart" => JsonConvert.DeserializeObject<Restart>(jObject.ToString())!,
                "IfBlock" => JsonConvert.DeserializeObject<IfBlock>(jObject.ToString())!,
                "SwitchBlock" => JsonConvert.DeserializeObject<SwitchBlock>(jObject.ToString())!,
                "Print" => JsonConvert.DeserializeObject<Print>(jObject.ToString())!,
                "Return" => JsonConvert.DeserializeObject<Return>(jObject.ToString())!,
                _ => throw new InvalidDataException("Invalid procedure element supplied in json"),
            };
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
        public override bool CanConvert(Type objectType) => objectType == typeof(ITerm);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                var list = new List<ITerm?>();
                foreach (var item in token)
                {
                    list.Add(ConvertElement((item as JObject)!, serializer));
                }
                return list;
            }

            return ConvertElement((token as JObject)!, serializer);
        }

        private static ITerm ConvertElement(JObject jObject, JsonSerializer serializer)
        {
            if (jObject == null)
                return null;
            string type = jObject["Type"]!.ToString();

            ITerm? term = type switch
            {
                "Varbuf" => JsonConvert.DeserializeObject<Varbuf>(jObject.ToString())!,
                "Constant" => JsonConvert.DeserializeObject<Constant>(jObject.ToString())!,
                "Literal" => JsonConvert.DeserializeObject<Literal>(jObject.ToString())!,
                "PassedArg" => JsonConvert.DeserializeObject<PassedArg>(jObject.ToString())!,
                "VariableArray" => JsonConvert.DeserializeObject<VariableArray>(jObject.ToString())!,
                "Linkvarbuf" => JsonConvert.DeserializeObject<Linkvarbuf>(jObject.ToString())!,
                "Localvarbuf" => JsonConvert.DeserializeObject<Localvarbuf>(jObject.ToString())!,
                "LocalVar" => JsonConvert.DeserializeObject<LocalVar>(jObject.ToString())!,
                "Procedure" => JsonConvert.DeserializeObject<Procedure>(jObject.ToString())!,
                "Expression" => JsonConvert.DeserializeObject<Expression>(jObject.ToString())!, //TODO: breaks on null Term1 >:C (fixed, i think?)
                _ => throw new InvalidDataException("Invalid term supplied in json"),
            };
            return term;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

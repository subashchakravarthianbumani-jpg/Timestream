using Model.ViewModel;
using Newtonsoft.Json;

namespace TIMEAPI
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            string ObjectString = Utils.EncryptDecrypt.Encrypt(JsonConvert.SerializeObject(value));

            session.SetString(key, ObjectString);
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>(Utils.EncryptDecrypt.Decrypt(value));
            }
            else
            {
                return default;
            }
        }
    }
}

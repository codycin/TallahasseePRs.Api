using System.Text;

namespace TallahasseePRs.Api.Common.Paging
{
    public static class CursorCodec
    {
        //First we encode the createdAt and Id to a string
        public static string Encode(DateTime createdAt, Guid id)
        {
            //Ensure correct subtype
            if(createdAt.Kind != DateTimeKind.Utc) 
                createdAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);

            var payload = $"{createdAt.Ticks}|{id}";
            //Encode for security
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

        }
        //Now we add a decoder

        public static bool TryDecode(string? cursor, out DateTime createdAt, out Guid id)
        {
            createdAt = default;
            id = default;

            if (string.IsNullOrWhiteSpace(cursor)) return false;

            try
            {
                var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                var parts = raw.Split('|');
                if (parts.Length != 2) return false;

                if (!long.TryParse(parts[0], out var ticks)) return false;
                if (!Guid.TryParse(parts[1], out id)) return false;

                createdAt = new DateTime(ticks, DateTimeKind.Utc);
                return true;
            }
            catch { return false; }
            
        }
    }
}

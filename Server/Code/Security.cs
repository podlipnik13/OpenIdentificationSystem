using JWT.Algorithms;
using JWT.Builder;

#pragma warning disable CS0618 //'HMACSHA256Algorithm' is obsolete: 'HMAC SHA based algorithms are not secure to protect modern web applications. Consider switching to RSASSA or ECDSA.'

namespace Server.Code;

public class Security {
	
	public static Response GetToken(OISContext context, UserPayload res) {	

        Response response = new();
        
        try {          
            
            User user = context.Users.FirstOrDefault(u => u.Email == res.Email && u.UserName == res.UserName &&
                u.Password == Cryptography.HashThis(res.Password));

            if(user == null) throw new Exception("Invalid credentials");
            if(user.UserStatus == UserStatus.Declined) throw new Exception("User declined");

            DateTimeOffset expiryDate = DateTimeOffset.UtcNow.AddDays(1);
            response.Data = new JsonToken(){
                Token = JwtBuilder.Create()
            	            .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric A -(Secret)> B
            	            .WithSecret("GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk")
            	            .AddClaim("Expire", expiryDate.ToUnixTimeSeconds()) // addminutes
                            .AddClaim("UserId", user.Id)
            	            .Encode(),
                Expires = expiryDate
            };

            response.Status = ResponseStatus.OK;

        } catch (Exception ex) {
            response.Data = null;
            response.Message = ex.Message.ToString();
            response.Status = ResponseStatus.Error;
        }
        return response;
	}

	public static Response Authenticate(string token) {
		
        Response response = new();
		
		try {
            
            var jwt = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric B -(Secret)> A
                .WithSecret("GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk")
                .MustVerifySignature()
                .Decode<IDictionary<string, object>>(token);
            
            if((long)jwt["Expire"] < DateTimeOffset.UtcNow.ToUnixTimeSeconds()){
                response.Message = "Token timed out";
                response.Status = ResponseStatus.TimedOut;
                return response;
            }
            
            response.Data = int.Parse(jwt["UserId"].ToString());
            response.Status = ResponseStatus.OK;

        }
        catch (Exception ex) {
            response.Data = null;
            response.Message = ex.ToString();
            response.Status = ResponseStatus.Error;
        }
        return response;
	}

}	
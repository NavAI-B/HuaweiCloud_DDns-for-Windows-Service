using HuaweiCloud.SDK.Core;
using HuaweiCloud.SDK.Core.Auth;
using HuaweiCloud.SDK.Dns.V2;
using HuaweiCloud.SDK.Dns.V2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HuaweiCloud_DDns
{
    internal class Client
    {
        private readonly DnsClient m_Client;

        public Client(string ak, string sk, string region)
        {
            var httpConfig = HttpConfig.GetDefaultConfig();
            httpConfig.IgnoreBodyForGetRequest = true;

            var auth = new BasicCredentials(ak, sk);

            m_Client = DnsClient.NewBuilder()
              .WithCredential(auth)
              .WithRegion(DnsRegion.ValueOf(region))
              .WithHttpConfig(httpConfig)
              .Build();
        }

        public List<PublicZoneResp> GetPublicZones()
        {
            var zones = new List<PublicZoneResp>();
            var request = new ListPublicZonesRequest();
            try
            {
                var response = m_Client.ListPublicZones(request);
                Debug.WriteLine(response.ToString());
                zones.AddRange(response.Zones);
            }
            catch (RequestTimeoutException requestTimeoutException)
            {
                Debug.WriteLine(requestTimeoutException.ErrorMessage);
            }
            catch (ServiceResponseException clientRequestException)
            {
                Debug.WriteLine(clientRequestException.HttpStatusCode);
                Debug.WriteLine(clientRequestException.RequestId);
                Debug.WriteLine(clientRequestException.ErrorCode);
                Debug.WriteLine(clientRequestException.ErrorMsg);
            }
            catch (ConnectionException connectionException)
            {
                Debug.WriteLine(connectionException.ErrorMessage);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return zones;
        }

        public List<ListRecordSets> GetRecorderSets(string zoneId)
        {
            var recordsets = new List<ListRecordSets>();
            var request = new ListRecordSetsByZoneRequest { ZoneId = zoneId };

            try
            {
                var response = m_Client.ListRecordSetsByZone(request);
                Debug.WriteLine(response.ToString());
                recordsets.AddRange(response.Recordsets);
            }
            catch (RequestTimeoutException requestTimeoutException)
            {
                Debug.WriteLine(requestTimeoutException.ErrorMessage);
            }
            catch (ServiceResponseException clientRequestException)
            {
                Debug.WriteLine(clientRequestException.HttpStatusCode);
                Debug.WriteLine(clientRequestException.RequestId);
                Debug.WriteLine(clientRequestException.ErrorCode);
                Debug.WriteLine(clientRequestException.ErrorMsg);
            }
            catch (ConnectionException connectionException)
            {
                Debug.WriteLine(connectionException.ErrorMessage);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return recordsets;
        }

        public void SetRecord(UpdateRecordSetRequest request)
        {
            try
            {
                var response = m_Client.UpdateRecordSet(request);
                Debug.WriteLine(response.ToString());
            }
            catch (RequestTimeoutException requestTimeoutException)
            {
                Debug.WriteLine(requestTimeoutException.ErrorMessage);
            }
            catch (ServiceResponseException clientRequestException)
            {
                Debug.WriteLine(clientRequestException.HttpStatusCode);
                Debug.WriteLine(clientRequestException.RequestId);
                Debug.WriteLine(clientRequestException.ErrorCode);
                Debug.WriteLine(clientRequestException.ErrorMsg);
            }
            catch (ConnectionException connectionException)
            {
                Debug.WriteLine(connectionException.ErrorMessage);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}

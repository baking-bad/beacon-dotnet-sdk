using System;

namespace BeaconSdk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MatrixSdk.Application;

    public class CommunicationClient
    {
        private List<MatrixClient> matrixClients;
        public CommunicationClient(List<MatrixClient> matrixClients)
        {
            this.matrixClients = matrixClients;
        }

        public async Task StartAsync()
        {
            
        }
    }
}
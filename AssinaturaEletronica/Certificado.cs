using System;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace AssinaturaEletronica
{
    public class Certificado
    {
        public AsymmetricKeyParameter Key { get; private set; }

        public X509Certificate[] Chain { get; private set; }

        public Certificado(string caminhoPfx, string chavePfx = null)
        {
            using (var file = File.OpenRead(caminhoPfx)) {
                var password = chavePfx.ToCharArray();
                var store = new Pkcs12Store(file, password);
                var alias = GetCertificateAlias(store);

                Key = store.GetKey(alias).Key;

                Console.Write(Key);
                Chain = store.GetCertificateChain(alias).Select(x => x.Certificate).ToArray();
            }
        }

        private static string GetCertificateAlias(Pkcs12Store store)
        {
            foreach (string currentAlias in store.Aliases)
            {
                if (store.IsKeyEntry(currentAlias))
                {
                    return currentAlias;
                }
            }

            return null;
        }
    }
}
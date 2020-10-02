using System;
using System.Diagnostics;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;

namespace AssinaturaEletronica
{
    public class Certificador
    {
        private readonly Certificado certificado;

        public Certificador()
        {
           
        }

        public bool CertificarDocumento(string caminhoDocAssinado)
        {
            using (var reader = new PdfReader(caminhoDocAssinado))
            {
                var campos = reader.AcroFields;
                var nomeAssinaturas = campos.GetSignatureNames();
                foreach (var nome in nomeAssinaturas)
                {
                    if (ValidarAssinatura(campos, nome))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ValidarAssinatura(AcroFields campos, string nome)
        {
            // Só verifica a última revisão do documento.
            if (campos.GetRevision(nome) != campos.TotalRevisions)
                return false;

            // Só verifica se a assinatura é do documento todo.
            if (!campos.SignatureCoversWholeDocument(nome))
                return false;

            var assinatura = campos.VerifySignature(nome);

            if (!assinatura.Verify())
                return false;

            foreach (var certificadoDocumento in assinatura.Certificates)
            {

                foreach (var certificadoDeConfianca in certificado.Chain)
                {
                    try
                    {
                        certificadoDocumento.Verify(certificadoDeConfianca.GetPublicKey());
                        // Só entra aqui se a ultima assinatura foi assinada com certificado

                        return true;
                    }
                    catch (InvalidKeyException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Error: {0}", ex);
                        continue;
                    }
                }
            }

            return false;
        }
    }
}
using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

using System.Collections.Generic;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using iTextSharp.text;

namespace AssinaturaEletronica
{
    public class Assinante
    {
        private readonly Certificado certificado;

        public Assinante()
        {
            this.certificado = null;
        }
       


        public void Assinar(string caminhoDocSemAssinatura, string caminhoDocAssinado)
        {
            try
            {
                X509Store store = new X509Store(StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection sel = X509Certificate2UI.SelectFromCollection(store.Certificates, "Assinatura Digital", "Escolha uma assinatura abaixo:", X509SelectionFlag.SingleSelection);

                X509Certificate2 cert = sel[0];

                Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
                Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] {
            cp.ReadCertificate(cert.RawData)};
                //Console.Write("ANTES");
                //IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA-1");
                //Console.Write("aqui");

                //Get Cert Chain
                IList<Org.BouncyCastle.X509.X509Certificate> signatureChain = new List<Org.BouncyCastle.X509.X509Certificate>();
                X509Chain x509chain = new X509Chain();
                x509chain.Build(cert);
                foreach (X509ChainElement x509ChainElement in x509chain.ChainElements)
                {
                    signatureChain.Add(DotNetUtilities.FromX509Certificate(x509ChainElement.Certificate));
                }


                using (var reader = new PdfReader(caminhoDocSemAssinatura))
                using (var writer = new FileStream(caminhoDocAssinado, FileMode.Create, FileAccess.Write))
                using (var stamper = PdfStamper.CreateSignature(reader, writer, '\0', null, true))
                {
                    PdfSignatureAppearance signature = stamper.SignatureAppearance;
                    signature.CertificationLevel = PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED;
                    signature.Reason = "Hospital Austa";
                    signature.ReasonCaption = "Tipo de Assinatura: ";

                    //Console.Write("ANTES");
                    //Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair key = DotNetUtilities.GetKeyPair(cert.PrivateKey);
                    //Console.Write("ANTES2");
                    // var signatureKey = new PrivateKeySignature(cert.PrivateKey, "SHA256");
                    //Console.Write("depois");

                    var parser = new Org.BouncyCastle.X509.X509CertificateParser();
                    var bouncyCertificate = parser.ReadCertificate(cert.RawData);
                    var algorithm = DigestAlgorithms.GetDigest(bouncyCertificate.SigAlgOid);
                    var signatureKey = new X509Certificate2Signature(cert, algorithm);



                    //signatureChain = cert;
                    var standard = CryptoStandard.CADES;
                    
                    signature.SignatureGraphic = Image.GetInstance(@"\\192.168.10.27\a3\certificado.jpg");
                    signature.SetVisibleSignature(new Rectangle(100, 100, 250, 150), reader.NumberOfPages, "Signature");
                    signature.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;

                    MakeSignature.SignDetached(signature, signatureKey, signatureChain, null, null, null, 0, CryptoStandard.CMS);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            /*signatureAppearance.SignatureGraphic = Image.GetInstance(pathToSignatureImage);
            signatureAppearance.SetVisibleSignature(new Rectangle(100, 100, 250, 150), pdfReader.NumberOfPages, "Signature");
            signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;

            MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);*/
        }
    }
}
// See https://aka.ms/new-console-template for more information
using Savoia.Desencalha.Common.Util;
namespace Criptografia
{
    class Program
    {
        static void Main(string[] args)
        {


            while (true)
            {

                Console.Write("Tipo criptografia Senha ou Texto (S ou T): ");
                var tipoCriptografia = Console.ReadLine();

                if (!(tipoCriptografia == "S" || tipoCriptografia == "T")) { Sair(); continue; }

                if (tipoCriptografia == "T")
                {
                    Console.Write("Criptografar ou Descriptografar ? (C ou D): ");
                    var tipo = Console.ReadLine();
                    if (!(tipo == "C" || tipo == "D")) { Sair(); continue; }

                    Console.Write("Digite: ");
                    var texto = Console.ReadLine();

                    Console.WriteLine((tipo == "C" ? CriptografiaUtil.EncriptarPadrao(texto) : CriptografiaUtil.DecriptarPadrao(texto)));
                }
                else
                {
                    Console.Write("Digite o Salt: ");
                    var salt = Console.ReadLine();

                    Console.Write("Criptografar ou Comparar ? (1 ou 2): ");
                    var tipo = Console.ReadLine();

                    if (!(tipo == "1" || tipo == "2")) { Sair(); continue; }


                    string resultado;
                    if (tipo == "1")
                    {
                        Console.Write("Digite a senha para criptografar: ");
                        var texto = Console.ReadLine();

                        resultado = CriptografiaUtil.CriptografarSenha(texto, salt);
                    }
                    else
                    {
                        Console.Write("Digite a senha sem criptografia: ");
                        var senha = Console.ReadLine();

                        Console.Write("Digite o hash da senha: ");
                        var hashSenha = Console.ReadLine();

                        resultado = CriptografiaUtil.ValidarSenha(senha, salt, hashSenha).ToString();
                    }

                    Console.WriteLine(resultado);

                    //   Console.WriteLine((tipo == "C" ? CriptografiaUtil.EncriptarPadrao(texto  + salt) : CriptografiaUtil.VerificarSenha(texto)));

                    Console.Read();
                }

            }

        }

        static void Sair()
        {
            Console.WriteLine("Opção Inválida");
            Console.Read();

        }

    }


}


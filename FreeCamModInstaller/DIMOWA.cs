using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using FCM;
using dnlib.DotNet.Emit;
using dnpatch;


namespace IMOWA
{
    class DIMOWA // Debuger e Instalador de Mods do Outer Wilds Alpha
    {

        //RETURN:
        // 0 - Deu tudo OK
        // 1 - Deu erro
        // 2 - Não Deu certo, mas não por erro

        static Target ModInnitTarget(Type modClass, string modName, Patcher p, string modMethodToTarget, string modClassToTarget, string modNamespaceToTarget = "", int indiceOfIntructions = 0)
        {
           

            //Padrão genérico das Instructions para os MOWA
            Instruction[] opcodesModInnit = {

                Instruction.Create(OpCodes.Ldstr   ,  $"{modName} foi iniciado | was started"),

                Instruction.Create(OpCodes.Call, p.BuildCall(typeof(UnityEngine.Debug), "Log" , typeof(void) , new[]{ typeof(object) })),

                Instruction.Create(OpCodes.Ldstr   ,  modName),

                Instruction.Create(OpCodes.Call, p.BuildCall(modClass, "ModInnit", typeof(void), new[] { typeof(string) }))
                

                };

            int[] indicesDoMod = new int[opcodesModInnit.Length];

            for (int i =0;i < indicesDoMod.Length;i++)
            {
                indicesDoMod[i] = i + indiceOfIntructions;
            }
                


            //Criar um target genérico que pode ser setado usando variaveis padrões da Classe MOWA
            Target targetMod = new Target()
            {
                Namespace = modNamespaceToTarget,
                Class = modClassToTarget, 
                Method = modMethodToTarget, 
                Instructions = opcodesModInnit,
                Indices = indicesDoMod,
                InsertInstructions = true

            };

            return targetMod;
        }

        static Target ModInnitTarget(MOWAP modMOWAP, Patcher modPatcher)
        {
            return ModInnitTarget(modMOWAP.ModType, modMOWAP.ModName, modPatcher, modMOWAP.ModMethodToTarget, modMOWAP.ModClassToTarget, modMOWAP.ModNamespaceToTarget, modMOWAP.IndiceOfIntructions);
        }

        //PORQUE ISSO NÃO PODE SER UM POINTER AHAHAHAH
        static int InstallMod(Target modInnitTarget, Patcher modPatcher)
        {
            
            try
            {
                modPatcher.Patch(modInnitTarget);

                Console.WriteLine("Os Patchings foram um sucesso, salvando agora. . . | The Patchings were a success, saving now. . .");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Erro no Patching | Patching error: {exp}");
            }


            //try
            //{
                modPatcher.Save("Assembly-CSharp-ModLoaded.dll");
                Console.WriteLine("Mod Salvado com Sucesso :: ) | Mod saving was successfull :: )");
                return 0;
            //}
            //catch (Exception exp)
            //{
            //    Console.WriteLine($"Erro no Saving | Saving error: {exp}");
            //    Console.WriteLine("O mod não foi possivel de ser instalado | It wasn't possible to install the mod");
            //    return 1;
            //}

        }

        

        static int CheckIfModInstalled(Target modInnitTarget, Patcher modPatcher, int occurence = 0)
        {
            Instruction[] instructionOfMethod = modPatcher.GetInstructions(modInnitTarget);

            int comparingIndex = 0; // Do method para conferir
            int instructionIndex = 0; // Do mod

            int occurenceCounter = 0;

           

            while (instructionIndex < modInnitTarget.Instructions.Length && comparingIndex < instructionOfMethod.Length)
            {
                if (modInnitTarget.Instructions[instructionIndex].Operand == null && instructionOfMethod[comparingIndex].Operand == null)
                {
                    
                    if (modInnitTarget.Instructions[instructionIndex].OpCode.Name == instructionOfMethod[comparingIndex].OpCode.Name)
                    {
						comparingIndex++;
						instructionIndex++;
                       

                        if (instructionIndex == (modInnitTarget.Instructions.Length - 1))
                        {
                           

                            if (occurenceCounter == occurence)
                                return comparingIndex - instructionIndex;
                            else
                                occurenceCounter++;
                        }

                    }
                }
                else if (modInnitTarget.Instructions[instructionIndex].OpCode.Name == instructionOfMethod[comparingIndex].OpCode.Name && modInnitTarget.Instructions[instructionIndex].Operand.ToString() == instructionOfMethod[comparingIndex].Operand.ToString())
                {
                    
                    comparingIndex++;
					instructionIndex++;

                    if(instructionIndex == (modInnitTarget.Instructions.Length - 1))
                    {
                        

                        if (occurenceCounter == occurence)
                            return comparingIndex - instructionIndex;
                        else
                            occurenceCounter++;
                    }


                }
				else
                {
                    
                    comparingIndex = comparingIndex - instructionIndex + 1;
                    instructionIndex = 0;
                }
            }

            
            return -1;
        }

        static int UninstallMod(Target modInnitTarget, Patcher modPatcher, int modAlreadyInstalledIndex)
        {
            

             for(int i =0; i < modInnitTarget.Indices.Length; i++)
                {
                    modInnitTarget.Indices[i] += modAlreadyInstalledIndex;
                }
                modPatcher.RemoveInstruction(modInnitTarget);

            //Não precisa de backup, né?
            modPatcher.Save(false);

                return 0;
            
        }

            static void Main(string[] args)
        {

            //Colocando o struct para coisas legais e tals
            MOWAP freeCamMod = new MOWAP
            {
                ModType = typeof(FCM.FreeCamMod),

                ModName = "FreeCamMod",

                ModMethodToTarget = "Awake",

                ModClassToTarget = "PlayerBody",

                ModNamespaceToTarget = "",

                IndiceOfIntructions = 0,

            };


            // Se o mod já está instalado...não instalar


            Console.WriteLine(" --- DIMOWA v.2 --- ");
            Patcher patcher = new Patcher("Assembly-CSharp.dll");

            Target freeCamModInnitTarget = ModInnitTarget(freeCamMod,patcher);

            //Não! não é para ser um bollean, acho que da para entender o porque

            
            int isModInstalled = CheckIfModInstalled(freeCamModInnitTarget, patcher);

            if(isModInstalled >= 0)
            {
                
                Console.WriteLine($"O mod {freeCamMod.ModName} já está instalado, voce gostaria desinstala-lo? s / n");
                Console.WriteLine($"The mod {freeCamMod.ModName} is already installed, would you like to uninstall it? y / n");
                int resposta = Console.Read();

                if (resposta == 'y' || resposta == 's')
                {
                    Console.WriteLine('\n' + "Desinstalando| Uninstalling . . .");

                    int modIsUnistalled = UninstallMod(freeCamModInnitTarget, patcher, isModInstalled);

                    if (modIsUnistalled == 0)
                    {
                        Console.WriteLine("O mod foi desinstalado com sucesso");
                        Console.WriteLine("The mod was sucesffuly unistalled");
                    }
                    else
                    {
                        Console.WriteLine("O mod não foi desinstalado");
                        Console.WriteLine("The mod was not uninstalled");
                    }
                }
               
            }
            else 
            {
                Console.WriteLine($"O mod {freeCamMod.ModName} nao esta instalado, deseja instala-lo? s / n");
                Console.WriteLine($"The mod {freeCamMod.ModName} is not installed, would you like to install it? y / n");
                int resposta = Console.Read();

                if (resposta == 'y' || resposta == 's')
                {
                    Console.WriteLine($"Instalando o mod | Instaling the mod: {freeCamMod.ModName}");


                    

                    int modIsIstalled = InstallMod(freeCamModInnitTarget, patcher);

                    if (modIsIstalled == 0)
                    {
                        Console.WriteLine("O mod foi instalado com sucesso");
                        Console.WriteLine("The mod was sucesffuly installed");
                    }
                    else
                    {
                        Console.WriteLine("O mod não foi instalado");
                        Console.WriteLine("The mod was not istalled");
                    }
                }
            }


            Console.Read();
        }
    }
}

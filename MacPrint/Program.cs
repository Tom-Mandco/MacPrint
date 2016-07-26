namespace MacPrint
{
    using Ninject;
    using Interfaces;
    using Classes;

    class Program
    {
         //Mandatory Arguments For simple File:
         //File Path {C:\phd\cgirvan_wh550_r1_0128150928} args[0]
         //Is orientation landscape {true/false}          args[1]
         //Printert network path {\\ms35\ricoh7}          args[2]
         //Page size {A4,A3}                              args[3]
         //font size {int}10                              args[4]
         //SourseName {string}                            args[5]
         //is doublex {true/false}                        args[6]  
         //PrintBlankLines {true/false}                   args[7]

          //Mandatory Arguments For PDF File:        
          //file path "C:\Folder\File.pdf"                           args[0]
          //printer name "\\ms35\ricoh7" || \\ms35\lexmark32         args[1]
          //print with adobe {true/false}                            args[2] Use 'true' for older printers with full network name \\ms35\lexmark32
          
          //  args = new string[3];
          //  args[0] = "C:\\TestFiles\\R22_Kids_Girls_PSales_R_A_Dunn.pdf";
          //  args[1] = @"\\ms35\it";
          //  args[2] = "true";* 

        static void Main(string[] args)
        {
            CompositionRoot.Wire(new ApplicationModule());

            var printUtils = CompositionRoot.Resolve<IApp>();

            printUtils.Print(args);
        }
    }
}

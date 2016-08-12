﻿using System;
using System.IO;
using Baseline;
using Oakton.Parsing;
using Shouldly;
using Xunit;

namespace Oakton.Testing
{
    public class OptionReaderTester
    {
        [Fact]
        public void read_from_one_line()
        {
            var system = new FileSystem();


            var path = AppContext.BaseDirectory.AppendPath("opts1.txt");
            system.WriteStringToFile(path, "-f -a -b");

            OptionReader.Read(path)
                .ShouldBe("-f -a -b");
        }

        [Fact]
        public void read_from_multiple_lines()
        {
            var path = AppContext.BaseDirectory.AppendPath("opts2.txt");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                var writer = new StreamWriter(stream);

                writer.WriteLine("--color Blue");
                writer.WriteLine("--size Medium    ");
                writer.WriteLine("   --direction East");

                writer.Flush();

                writer.Dispose();
            }

            OptionReader.Read(path)
                .ShouldBe("--color Blue --size Medium --direction East");
        }
    }
}
using System;
using Xunit;
using PerpustakaanApp.Models;

namespace soal1.Tests
{
    public class BukuTests
    {
        [Fact]
        public void Buku_Constructor_ShouldSetProperties()
        {
            // Arrange
            int expectedId = 1;
            string expectedJudul = "C# Programming";
            string expectedPenulis = "John Doe";

            // Act
            Buku buku = new Buku(expectedId, expectedJudul, expectedPenulis);

            // Assert
            Assert.Equal(expectedId, buku.Id);
            Assert.Equal(expectedJudul, buku.Judul);
            Assert.Equal(expectedPenulis, buku.Penulis);
        }

        [Fact]
        public void UpdateInfo_ShouldUpdateJudulAndPenulis()
        {
            // Arrange
            Buku buku = new Buku(1, "Judul Lama", "Penulis Lama");

            // Act
            buku.UpdateInfo("Judul Baru", "Penulis Baru");

            // Assert
            Assert.Equal("Judul Baru", buku.Judul);
            Assert.Equal("Penulis Baru", buku.Penulis);
        }
    }
}

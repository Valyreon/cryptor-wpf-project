<img align="right" src="https://github.com/Valyreon/cryptor-wpf-project/blob/master/FileEncryptorWpf/cryptLogoSmaller.PNG?raw=true"><img>
# Cryptor

**Cryptor** is a program written in **C#** that can encrypt and decrypt files using hybrid data encryption. It has a 
built in **PKI** and can encrypt files using several encryption and hashing algorithms.

Cryptor works by generating a random password or key for the encryption algorithm, encrypting the data block by block and then encrypting the encryption key using
the receivers public key and writing it all in a **custom format** file. That file is then digitally signed using the senders private key and the signature is appended to
the file. To decrypt, receiver only needs to have his **private key** which matches the public key used by the sender. Because it encrypts files by dividing it into smaller
blocks, it can be used for encryption very large files. Speed also depends on algorithms used.

This software is modification of a project I did for a course **Cryptography and Computer Protection** at my University. I had fun developing it. It can be used
as a reference for **MVVM (Model-View-ViewModel)** architecture and **X.509 Certificates**.

### Technologies

Cryptor is written in **C#** and requires **[.NET Framework 4.7.2](https://dotnet.microsoft.com/download/thank-you/net472)** to run. It was developed in Visual Studio Community 2019.
Application's user interface is implemented using **Windows Presentation Foundation (WPF)** and it uses **[SQLite](https://www.sqlite.org/index.html)** with **Entity Framework 6**
to store data.

Also for the implementation of the 'Twofish' algorithm I used the **[BouncyCastle](https://www.nuget.org/packages/BouncyCastle/)** Nuget package. I would like to thank Bouncy Castle 
developers for the great work they did developing it.

### Algorithms

#### Encryption

Cryptor for now supports three encryption algorithms: 
* Advanced Encryption Standard (AES)
* Triple DES
* Twofish
and is easily extendible thanks to the MVVM Architecture.

**RSA algorithm** is used for key encryption and generating a digital signature of the file.

#### Hashing

For hashing there are two algorithms the application can use:
* SHA256
* MD5
and it's also easy to implement more of them.

### Implementation

The solution consists of 7 projects. 

Project **AlgorithmLibrary** contains implementation for encryption algorithms like **AES**, **Twofish** and **TripleDES**. Every algorithm 'machine' should
implement the **IMachine** interface so the application stays easily extensible with new algorithms by using **polymorphism**. This project also used to contain implementation of hash algorithms but I 
decided it was unneccessary so custom hashing algorithms should implement the abstract class **HashAlgorithm** which is included in .NET in **System.Security.Cryptography** package. Hashing
algorithms I were already implemented in .NET.
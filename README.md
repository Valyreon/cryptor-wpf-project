<img align="right" src="https://github.com/Valyreon/cryptor-wpf-project/blob/master/FileEncryptorWpf/cryptLogoSmaller.PNG?raw=true"><img>
# Cryptor

**Cryptor** is a program written in **C#** that can encrypt and decrypt files using hybrid data encryption. It has a 
built in **PKI** and can encrypt files using several encryption and hashing algorithms.

Cryptor works by generating a random password or key for the encryption algorithm, encrypting the data block by block and then encrypting the encryption key using
the receivers public key and writing it all in a file with **custom format**. That file is then digitally signed using the senders private key and the signature is appended to
the file. To decrypt, receiver only needs to have his **private key** which matches the public key used by the sender. Because it encrypts files by dividing it into smaller
blocks, it can be used for encrypting very large files. Speed depends on algorithms used.

This software is modification of a project I did for a course **Cryptography and Computer Protection** at my University. I had fun developing it. It can be used
as a reference for **MVVM (Model-View-ViewModel)** architecture and **X.509 Certificates**.

### Technologies
---
Cryptor is written in **C#** and requires **[.NET Framework 4.7.2](https://dotnet.microsoft.com/download/thank-you/net472)** to run. It was developed in Visual Studio Community 2019.
Application's user interface is implemented using **Windows Presentation Foundation (WPF)** and it uses **[SQLite](https://www.sqlite.org/index.html)** with **Entity Framework 6**
to store data.

Also for the implementation of the 'Twofish' algorithm I used the **[BouncyCastle](https://www.nuget.org/packages/BouncyCastle/)** Nuget package. I would like to thank Bouncy Castle 
developers for the great work they did developing it.

### Algorithms
---

Cryptor for now supports three encryption algorithms: 
* Advanced Encryption Standard (AES)
* Triple DES
* Twofish

**RSA cryptosystem** is used for key encryption and generating a digital signature of the file.

For hashing there are two algorithms the application can use:
* SHA256
* MD5

Because the application uses **polymorphism** and **MVVM** architecture it would be easy to further expand the collection of supported algorithms by implementing correct interfaces.

### Usage
---
To use the application user first needs to register. To do that, he needs to provide the system with an **Username**, **Password** and his **X.509 Public Certificate** file.
After that he can login to the system. Same registering form is used for adding external users, those users that you want to send encrypted files to. In that case the user needs to check
the **External** checkbox and then he can register an external user by providing only **Username** you wish to identify him with and his **Public Certificate**.

At login, user needs to provide his **Username**, **Password** and his **Private Key** that matches the Public Key included in his Public Certificate he used to register. The application will
check if the private key matches the public key, and it will only login if they match.

In the main form, user can set all the desired parameters: **input file**, **output file**, **algorithms** to use, **mode** and **sender**/**receiver**. If mode of operation is decryption, system will deduce the algorithms
used from the header of the encrypted file.

During encryption or encryption user will be presented with a progress bar and a text area where application will log it's progress.

#### Testing

For testing purposes there is a folder **'OPENSSL'** and a database **Users.db** provided in the repository with certificates and private keys. There are two users already registered:
1. *Username*: default; *Password*: default; *Key*: OPENSSL/private/default.key
2. *Username*: luka; *Password*: luka; *Key*: OPENSSL/private/luka.key

If for some reason you want to create a new SQLite database here is the proper query:
~~~~
CREATE TABLE Users (
    Id                INTEGER   PRIMARY KEY AUTOINCREMENT
                                UNIQUE,
    Username          CHAR (20) NOT NULL
                                UNIQUE,
    Salt              BLOB,
    PassHash          BLOB,
    IsExternal        INT       NOT NULL,
    PublicCertificate BLOB      NOT NULL
);
~~~~

### Implementation
---
Project **AlgorithmLibrary** contains implementation for encryption algorithms like **AES**, **Twofish** and **TripleDES**. Every algorithm 'machine' should
implement the **IMachine** interface so the application stays easily extensible with new algorithms by using **polymorphism**. This project should contain hash algorithms that implement 
the abstract class System.Security.Cryptography.**HashAlgorithm**. Hashing algorithms I used were already implemented in .NET.

**CryptedStreamParsers** project contains all neccessary classes for encrypting and decrypting streams. Every class here works with streams, so encrypted data doesn't neccessarilly needs to be
a file, it can be a memory stream or something else. **PrivateKeyParsers** project contains neccessary classes for reading private key files.

Project **FileEncryptorWpf** contains all the Models, Views and ViewModels. Every ViewModel inherits the abstract class
ViewModelBase which implements interfaces **INotifyDataErrorInfo** and **INotifyPropertyChanged**. *INotifyDataErrorInfo* provides
custom synchronous and asynchronous validation support and *INotifyPropertyChanged* which is used for notifying Views that a property value has changed.

### Screenshots
---

<img align="center" src="https://github.com/Valyreon/cryptor-wpf-project/blob/master/cryptor-main-form.PNG?raw=true"><img>
<img src="https://github.com/Valyreon/cryptor-wpf-project/blob/master/cryptor-login.PNG?raw=true"><img>
<img src="https://github.com/Valyreon/cryptor-wpf-project/blob/master/cryptor-output.PNG?raw=true"><img>

----
This free software was developed by <i><strong><a href="https://www.linkedin.com/in/luka-budrak/">Luka B.</a></strong></i>
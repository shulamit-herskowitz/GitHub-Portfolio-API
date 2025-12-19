# GitHub Portfolio API 🚀

**A .NET 9 Web API that displays a developer's GitHub portfolio with a modern, responsive frontend.**

---

## 📖 **Project Overview**

This project connects to a GitHub account and displays:
- List of repositories
- Programming languages used
- Last commit date
- Stars count
- Pull requests count
- Repository homepage links

Additionally, it allows searching public GitHub repositories with filters for:
- Repository name
- Programming language
- Username

---

## 🏗️ **Architecture**

The project is organized into two main components:

### **1. GitHubCV.Services** (Class Library)
- `IGitHubService` - Service interface
- `GitHubService` - Core GitHub integration using Octokit
- `CachedGitHubService` - Decorator pattern with in-memory caching (5-minute expiration)
- `GitHubSettings` - Configuration model for user secrets

### **2. GitHubCV.Api** (Web API)
- RESTful API endpoints
- Static file serving for frontend
- Dependency Injection with Options Pattern
- Scrutor for decorator registration

---

## ✨ **Features**

✅ **Options Pattern** - Secure credential injection with `IOptions<GitHubSettings>`  
✅ **Decorator Pattern** - In-memory caching with Scrutor  
✅ **Octokit Integration** - Official GitHub API client  
✅ **User Secrets** - Secure token storage (not in code!)  
✅ **Modern Frontend** - Tailwind CSS, Dark Mode, Responsive  
✅ **Search Functionality** - Filter by name, language, and user  

---

## 🚀 **Getting Started**

### **Prerequisites**
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- GitHub account
- [Personal Access Token](https://github.com/settings/tokens)

### **1. Clone the Repository**
```sh
git clone https://github.com/shulamit-herskowitz/GitHub-Portfolio-API.git
cd GitHub-Portfolio-API
```

### **2. Configure User Secrets**
```sh
cd GitHubCV.Api
dotnet user-secrets init
dotnet user-secrets set "GitHubSettings:Username" "your-github-username"
dotnet user-secrets set "GitHubSettings:Token" "ghp_YourPersonalAccessToken"
```

**⚠️ Never commit your token to Git!**

### **3. Install Dependencies**
```sh
dotnet restore
```

### **4. Build the Project**
```sh
dotnet build
```

### **5. Run the Application**
```sh
dotnet run --project GitHubCV.Api
```

The app will be available at: `https://localhost:7168` (or the port shown in the console)

---

## 📡 **API Endpoints**

### **GET** `/api/portfolio`
Returns your GitHub repositories with full details.

**Response:**
```json
[
  {
    "name": "MyProject",
    "description": "A cool project",
    "url": "https://github.com/user/MyProject",
    "homepage": "https://myproject.com",
    "stars": 42,
    "pullRequestsCount": 5,
    "lastCommitDate": "2024-01-15T10:30:00Z",
    "languages": {
      "C#": 15000,
      "JavaScript": 5000
    }
  }
]
```

### **GET** `/api/portfolio/search?repoName={name}&language={lang}&user={user}`
Search public GitHub repositories.

**Query Parameters:**
- `repoName` (optional) - Repository name
- `language` (optional) - Programming language (e.g., `C#`, `JavaScript`)
- `user` (optional) - GitHub username

---

## 🛠️ **Technologies Used**

- **.NET 9** - Latest framework
- **Octokit** - GitHub API client
- **Scrutor** - Decorator pattern registration
- **Tailwind CSS** - Modern styling
- **In-Memory Cache** - Performance optimization

---

## 🔒 **Security**

✅ **User Secrets** - Tokens stored outside source code  
✅ **HTTPS Redirect** - Encrypted communication  
✅ **.gitignore** - Prevents committing sensitive files  
✅ **Options Pattern** - Professional configuration management  

---

## 📂 **Project Structure**

```
GitHubCV.Api/
├── Controllers/
│   └── PortfolioController.cs
├── wwwroot/
│   └── index.html
├── Program.cs
└── appsettings.json

GitHubCV.Services/
├── IGitHubService.cs
├── GitHubService.cs
├── CachedGitHubService.cs
├── GitHubSettings.cs
└── RepositoryInfo.cs
```

---

## 🎨 **Frontend**

The project includes a **beautiful dark-mode frontend** built with:
- Tailwind CSS (via CDN)
- Responsive grid layout
- Real-time search
- Animated cards
- Loading states

Access it at: `https://localhost:7168/`

---

## 🧪 **Testing**

### **Test the API:**
```sh
# Get your portfolio
curl https://localhost:7168/api/portfolio

# Search repositories
curl "https://localhost:7168/api/portfolio/search?repoName=test&language=CSharp"
```

---

## 📚 **References**

- [Octokit Documentation](https://octokitnet.readthedocs.io/)
- [Options Pattern in .NET](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)
- [Scrutor Decorator Pattern](https://github.com/khellang/Scrutor)
- [In-Memory Caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory)

---

## 👩‍💻 **Author**

**Shulamit Herskowitz**  
GitHub: [@shulamit-herskowitz](https://github.com/shulamit-herskowitz)

---

## 📄 **License**

This project is open source and available under the [MIT License](LICENSE).

---

**⭐ If you found this project helpful, please give it a star!**


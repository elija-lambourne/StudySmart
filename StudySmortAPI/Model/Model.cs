using System.Collections;

namespace StudySmortAPI.Model;

public record struct UserData(Guid Id, string JwtToken);
public record NotebookData(Guid Id,ICollection<string> Pages);
public record FolderData(Guid Id,string Name,ICollection<FolderData> ChildFolders,ICollection<Folder> ChildNotebooks);

public record struct DeadlineData(Guid Id, DateTime DateTimeUtc ,string Note, string Title);

public record FlashCardData(Guid Id,string Word,string Translation,int CorrectCnt,int WrongCnt,int SkipCnt, Guid CategoryId, string CategoryName);
public record struct FlashcardCategoryData(Guid Id,string Name,ICollection<FlashCardData> Flashcards);
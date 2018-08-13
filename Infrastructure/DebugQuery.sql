SELECT [vac].[Id], [vac].[FinalOffer], [vac].[InitOffer], [vac].[SubcategoryId], [vac].[Title], [vac].[Url], [subcat].[Id], [subcat].[CategoryId], [subcat].[Slug], [subcat].[Title], [subcat].[Url], [cat].[Id], [cat].[Slug], [cat].[Title], [cat].[Url]
FROM [Vacancies] AS [vac]
INNER JOIN [Subcategories] AS [subcat] ON [vac].[SubcategoryId] = [subcat].[Id]
INNER JOIN [Categories] AS [cat] ON [subcat].[CategoryId] = [cat].[Id]

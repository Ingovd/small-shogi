module Project where

import System.Directory
import Data.List
import ParseLib.Abstract.Applications
import ParseLib.Abstract.Core
import ParseLib.Abstract.Derived

dataDir = "C:\\Users\\Ingo\\Documents\\UU\\project\\Data"

data TestData = Succes {value :: Int, time :: Int, count :: Int} | Boring deriving (Show, Eq)
data Result = Black | White | Draw

main = do
        files <- getDirectoryContents dataDir
        
        putStrLn (intercalate "\n" files )
        
parseFile :: FilePath -> IO TestData
parseFile path = do
        content <- readFile path
        return Boring
        
        
run :: Parser a b -> [a] -> Maybe b
run p i = firstComplete (parse p i)
  where firstComplete []                = Nothing
        firstComplete ((res, rem) : xs) | null rem  = Just res
    	                                | otherwise = firstComplete xs

testDataParser :: Parser Char TestData
testDataParser = Succes <$ token "Type:BFS\nValue:" <*> integer <* token "Time:" <*> integer <* token "Count:" <*> integer <* (many.satisfy.const $ True)
import type { SearchResultsInitialState, SearchResultsStoreState } from '@sitecore-search/react';
import { WidgetDataType, useSearchResults, widget } from '@sitecore-search/react';

import ArticleItemCard from './ArticleHorizontalCard/index';
import Filters from './Filter/index';
import QueryResultsSummary from './QueryResultsSummary/index';
import ResultsPerPage from './ResultsPerPage/index';
import SearchFacets from './SearchFacets/index';
import SearchPagination from './SearchPagination/index';
import SortOptions from './SortOrder/index';
import Spinner from './Spinner/index';
import { GridStyled, PageControlsStyled, SearchResultsLayout } from './styled';

type ArticleModel = {
  id: string;
  author?: string;
  type?: string;
  title?: string;
  name?: string;
  subtitle?: string;
  url?: string;
  description?: string;
  content_text?: string;
  image_url?: string;
  source_id?: string;
};

type ArticleSearchResultsProps = {
  defaultSortType?: SearchResultsStoreState['sortType'];
  defaultPage?: SearchResultsStoreState['page'];
  defaultItemsPerPage?: SearchResultsStoreState['itemsPerPage'];
  defaultKeyphrase?: SearchResultsStoreState['keyphrase'];
};

type InitialState = SearchResultsInitialState<'itemsPerPage' | 'keyphrase' | 'page' | 'sortType'>;

export const SearchResultsComponent = ({
  defaultSortType = 'featured_desc',
  defaultPage = 1,
  defaultKeyphrase = '',
  defaultItemsPerPage = 24,
}: ArticleSearchResultsProps) => {
  const {
    widgetRef,
    actions: { onItemClick },
    state: { sortType, page, itemsPerPage },
    queryResult: {
      isLoading,
      isFetching,
      data: {
        total_item: totalItems = 0,
        sort: { choices: sortChoices = [] } = {},
        facet: facets = [],
        content: articles = [],
      } = {},
    },
  } = useSearchResults<ArticleModel, InitialState>({
    state: {
      sortType: defaultSortType,
      page: defaultPage,
      itemsPerPage: defaultItemsPerPage,
      keyphrase: defaultKeyphrase,
    },
  });
  const totalPages = Math.ceil(totalItems / itemsPerPage);
  if (isLoading) {
    return <Spinner />;
  }
  return (
    <SearchResultsLayout.Wrapper ref={widgetRef}>
      <SearchResultsLayout.MainArea>
        {isFetching && <Spinner />}
        {totalItems > 0 && (
          <>
            <SearchResultsLayout.LeftArea>
              <Filters />
              <SearchFacets facets={facets} />
            </SearchResultsLayout.LeftArea>
            <SearchResultsLayout.RightArea>
              {/* Sort Select */}
              <SearchResultsLayout.RightTopArea>
                {totalItems > 0 && (
                  <QueryResultsSummary
                    currentPage={page}
                    itemsPerPage={itemsPerPage}
                    totalItems={totalItems}
                    totalItemsReturned={articles.length}
                  />
                )}
                <SortOptions options={sortChoices} selected={sortType} />
              </SearchResultsLayout.RightTopArea>

              {/* Results */}
              <GridStyled>
                {articles.map((a, index) => (
                  <ArticleItemCard article={a} index={index} onItemClick={onItemClick} />
                ))}
              </GridStyled>
              <PageControlsStyled>
                <ResultsPerPage defaultItemsPerPage={defaultItemsPerPage} />
                <SearchPagination currentPage={page} totalPages={totalPages} />
              </PageControlsStyled>
            </SearchResultsLayout.RightArea>
          </>
        )}
        {totalItems <= 0 && !isFetching && (
          <SearchResultsLayout.NoResults>
            <h3>0 Results</h3>
          </SearchResultsLayout.NoResults>
        )}
      </SearchResultsLayout.MainArea>
    </SearchResultsLayout.Wrapper>
  );
};

const SearchResultsWidget = widget(SearchResultsComponent, WidgetDataType.SEARCH_RESULTS, 'content');

export default SearchResultsWidget;
import { styled } from 'styled-components';

import { theme } from '@sitecore-search/ui';

const Wrapper = styled.div``;

const MainArea = styled.div`
  color: ${theme.vars.palette.text.primary};
  font-family: ${theme.vars.typography.fontFamilySystem};
  font-size: ${theme.vars.typography.fontSize};
  padding: 0 ${theme.vars.spacing.m};
  display: flex;
  max-width: 100%;
  position: relative;
  padding: 0 ${theme.vars.spacing.m};
`;

const LeftArea = styled.section`
  display: flex;
  flex-direction: column;
  position: relative;
  flex: none;
  width: 25%;
  margin-right: ${theme.vars.spacing.l};
  margin-top: ${theme.vars.spacing.m};
`;

const RightArea = styled.section`
  display: flex;
  flex-direction: column;
  flex: 4 1 0%;
`;

const RightTopArea = styled.section`
  display: flex;
  justify-content: space-between;
`;

export const GridStyled = styled.div`
  width: 100%;
`;

export const PageControlsStyled = styled.div`
  display: flex;
  justify-content: space-between;
  font-family: ${theme.vars.typography.fontFamilySystem};
  font-size: ${theme.vars.typography.fontSize1.fontSize};
`;

export const NoResults = styled.div`
  display: flex;
  justify-content: center;
  width: 100%;
`;

export const SearchResultsLayout = {
  Wrapper,
  MainArea,
  NoResults,
  LeftArea,
  RightArea,
  RightTopArea,
};
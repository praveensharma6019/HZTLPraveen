import React from 'react';
import { Environment, WidgetsProvider } from '@sitecore-search/react';
import jssConfig from 'src/temp/config';

const BlogList = (): JSX.Element => {
  return (
    <section className="search-result max-w-screen-x1 mx-auto py-24 px-6">
      <WidgetsProvider
        apiKey="01-27fc34ac-ca5904109a03bae046cfde5b4a457cfbdb7cb05e"
        customerKey="94826063-93174088"
        env='test'
        publicSuffix={true}
      ></WidgetsProvider>
    </section>
  );
};
export default BlogList;
